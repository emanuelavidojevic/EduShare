<template>
  <div class="container-fluid py-2">
    <div class="d-flex align-items-center mb-4 pt-3">
      <div class="position-relative me-3">
        <img :src="`https://ui-avatars.com/api/?name=${userName || 'U'}+${userLastName || 'U'}&background=0d6efd&color=fff`" 
             alt="User Logo" 
             class="rounded-circle shadow-sm" 
             style="width: 65px; height: 65px; border: 3px solid white;">
        <span class="position-absolute bottom-0 end-0 badge border border-light rounded-circle bg-success p-2" style="transform: translate(-5%, -5%);">
          <span class="visually-hidden">Online</span>
        </span>
      </div>
      <div>
        <h3 class="fw-bold text-dark m-0">
          <span v-if="userName">Zdravo, {{ userName }}! 👋</span>
          <span v-else>Učitavanje...</span>
        </h3>
        <p class="text-muted m-0">Lepo je videti te ponovo na EduShare platformi.</p>
      </div>
    </div>

    <div class="mb-5">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h5 class="fw-bold m-0 text-secondary">Predstojeće obaveze</h5>
      </div>
      <div class="timeline-wrapper">
        <div class="timeline-scroll">
          <div class="timeline-card urgent">
            <div class="date-tag">Sutra</div>
            <h6 class="fw-bold mb-1 text-dark">Matematika 1: Kolokvijum</h6>
            <p class="small text-muted mb-0"><i class="bi bi-clock me-1"></i>09:00h - Sala 102</p>
          </div>
          <div class="timeline-card">
            <div class="date-tag text-primary">Petak</div>
            <h6 class="fw-bold mb-1 text-dark">CodeShare projekat</h6>
            <p class="small text-muted mb-0"><i class="bi bi-clock me-1"></i>Do ponoći</p>
          </div>
          <div class="timeline-card">
            <div class="date-tag text-success">Ponedeljak</div>
            <h6 class="fw-bold mb-1 text-dark">Prezentacija: Marketing</h6>
            <p class="small text-muted mb-0"><i class="bi bi-geo-alt me-1"></i>Kabinet 4</p>
          </div>
          <div class="timeline-card text-center d-flex align-items-center justify-content-center border-dashed">
            <span class="text-muted small">+ Dodaj podsetnik</span>
          </div>
        </div>
      </div>
    </div>

    <h3 class="fw-bold text-dark mb-4">Pregled aktivnosti</h3>

    <div class="card border-0 shadow-sm bg-white rounded-4 overflow-hidden">
      <div class="card-header bg-white border-0 py-3 px-4">
        <h5 class="fw-bold mb-0 text-secondary">Najskorije dodati materijali</h5>
      </div>
      <div class="table-responsive px-4 pb-4">
        <table class="table align-middle">
          <thead class="table-light">
            <tr>
              <th class="border-0 text-muted small text-uppercase">Title</th>
              <th class="border-0 text-muted small text-uppercase">Type</th>
              <th class="border-0 text-muted small text-uppercase">Author</th>
              <th class="border-0 text-muted small text-uppercase">Date</th>
              <th class="border-0 text-muted small text-uppercase text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-if="loading">
              <td colspan="5" class="text-center py-5">
                <div class="spinner-border spinner-border-sm text-primary me-2"></div>
                Učitavanje materijala...
              </td>
            </tr>

            <tr v-for="item in materials" :key="item.materialId">
              <td class="fw-bold text-primary">{{ item.name }}</td>
              <td>
                <span class="badge bg-light text-dark border">
                  {{ formatType(item.type) }}
                </span>
              </td>
              <td>{{ item.authorName }}</td>
              <td class="text-muted">{{ formatDate(item.dateCreated) }}</td>
              <td class="text-end">
                <div class="btn-group">
                  <button @click="downloadFile(item.materialId, item.name)" 
                          class="btn btn-sm btn-outline-primary border-0" title="Preuzmi">
                    <i class="bi bi-download"></i>
                  </button>
                  <button @click="deleteFile(item.materialId)" 
                          class="btn btn-sm btn-outline-danger border-0" title="Obriši">
                    <i class="bi bi-trash"></i>
                  </button>
                </div>
              </td>
            </tr>

            <tr v-if="!loading && materials.length === 0">
              <td colspan="5" class="text-center py-4 text-muted">Nema dostupnih materijala.</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const userName = ref('');
const userLastName = ref('');
const materials = ref([]);
const loading = ref(true);

const fetchUserSession = async () => {
  try {
    const res = await axios.get('http://localhost:5145/api/Auth/me', { withCredentials: true });
    if (res.data) {
      userName.value = res.data.firstName;
      userLastName.value = res.data.lastName;
    }
  } catch (e) {
    console.error("Korisnik nije prijavljen");
  }
};

const fetchMaterials = async () => {
  try {
    loading.value = true;
    // PROMENJENO: Izbačeno 'all-materials' jer kontroler koristi samo osnovnu rutu
    const res = await axios.get('http://localhost:5145/api/Material', { withCredentials: true });
    materials.value = res.data; 
  } catch (e) {
    console.error("Greška pri učitavanju materijala:", e);
  } finally {
    loading.value = false;
  }
};

const downloadFile = async (id, fileName) => {
  try {
    const response = await axios({
      url: `http://localhost:5145/api/Material/download/${id}`,
      method: 'GET',
      responseType: 'blob',
      withCredentials: true
    });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', fileName);
    document.body.appendChild(link);
    link.click();
    link.remove();
  } catch (e) {
    alert("Fajl se ne može preuzeti.");
  }
};

const deleteFile = async (id) => {
  if (!confirm("Da li želite da obrišete ovaj materijal?")) return;
  try {
    await axios.delete(`http://localhost:5145/api/Material/${id}`, { withCredentials: true });
    fetchMaterials(); // Ponovo učitaj listu
  } catch (e) {
    alert("Greška pri brisanju.");
  }
};

const formatDate = (dateString) => {
  if (!dateString) return "-";
  return new Date(dateString).toLocaleDateString('sr-RS', {
    day: '2-digit', month: '2-digit', year: 'numeric'
  });
};

const formatType = (type) => {
  const types = { 0: 'Others', 1: 'Pdf', 2: 'Video', 3: 'Image', 4: 'Document' };
  return types[type] || 'Others';
};

onMounted(async () => {
  await fetchUserSession();
  await fetchMaterials();
});
</script>

<style scoped>
.bg-success { background-color: #198754 !important; }
.timeline-wrapper { overflow-x: auto; scrollbar-width: none; padding-bottom: 10px; }
.timeline-wrapper::-webkit-scrollbar { display: none; }
.timeline-scroll { display: flex; gap: 1.2rem; }
.timeline-card { 
  min-width: 250px; 
  background: white; 
  padding: 1.2rem; 
  border-radius: 14px; 
  box-shadow: 0 4px 10px rgba(0,0,0,0.03); 
  border-top: 4px solid #dee2e6; 
}
.timeline-card.urgent { border-top-color: #dc3545; }
.border-dashed { border: 2px dashed #dee2e6; background: transparent; box-shadow: none; cursor: pointer; }
.date-tag { font-size: 0.75rem; font-weight: 800; text-transform: uppercase; margin-bottom: 5px; }
.table thead th { font-weight: 600; padding: 15px 10px; background-color: #f8f9fa; }
.table tbody td { padding: 15px 10px; border-bottom: 1px solid #f8f9fa; }
.rounded-4 { border-radius: 1rem !important; }
</style>