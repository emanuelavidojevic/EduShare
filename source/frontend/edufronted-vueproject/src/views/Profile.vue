<template>
  <div class="container-fluid py-4 bg-light min-vh-100">
    <div class="row g-4 mb-4"> 
      <div class="col-lg-5 col-xl-4">
        <div class="card border-0 shadow-sm h-100 rounded-4 overflow-hidden">
          <div class="card-body p-4 text-center d-flex flex-column justify-content-center">
            <div class="avatar-container mx-auto mb-3">
              <div class="avatar-box bg-primary shadow-sm">
                <i class="bi bi-person-fill text-white fs-1"></i>
              </div>
            </div>
            <h4 class="fw-bold mb-1">{{ currentUser.firstName }} {{ currentUser.lastName }}</h4>
            <p class="text-muted small mb-3 text-uppercase fw-bold">{{ currentUser.role }}</p>
            
            <div class="info-list text-start">
              <div class="info-item border-bottom py-2">
                <span class="text-muted tiny-label d-block text-uppercase">Email adresa</span>
                <span class="fw-semibold text-dark d-block text-truncate">{{ currentUser.email || 'Nema podataka' }}</span>
              </div>
              <div class="info-item border-bottom py-2">
                <span class="text-muted tiny-label d-block text-uppercase">Smer / Departman</span>
                <span class="fw-semibold text-dark d-block text-truncate">{{ currentUser.departmentName }}</span>
              </div>
              <div class="info-item pt-3 text-center">
                <span class="badge bg-success-subtle text-success rounded-pill px-3 py-2 border border-success-subtle">
                  <i class="bi bi-patch-check-fill me-1"></i> Aktivan Nalog
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-7 col-xl-8">
        <div class="card border-0 shadow-sm h-100 rounded-4 p-4 bg-white">
          <h5 class="fw-bold mb-4 text-primary"><i class="bi bi-bar-chart-fill me-2"></i>Aktivnost po predmetima</h5>
          <div v-if="loading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
          </div>
          <div v-else class="activity-chart">
            <div v-for="(item, index) in subjectStats" :key="index" class="mb-3">
              <div class="d-flex justify-content-between mb-1 small">
                <span class="fw-bold text-dark text-truncate me-2">{{ item.label }}</span>
                <span class="badge bg-light text-dark border flex-shrink-0">{{ item.count }} fajlova</span>
              </div>
              <div class="progress rounded-pill" style="height: 10px; background-color: #f0f0f0;">
                <div class="progress-bar rounded-pill shadow-sm" :style="{ width: item.percentage + '%', backgroundColor: colors[index % colors.length] }"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="row g-4 mb-4">
      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100">
          <h5 class="fw-bold mb-4 text-dark"><i class="bi bi-pie-chart-fill me-2 text-primary"></i>Zastupljenost predmeta</h5>
          <div class="h-100 d-flex flex-column justify-content-center">
            <div class="row align-items-center g-0">
              <div class="col-6 d-flex justify-content-center">
                <div class="donut-container">
                  <div class="donut-chart shadow-sm" :style="donutGradient">
                    <div class="donut-center">
                      <span class="fs-2 fw-bold text-dark">{{ totalMaterials }}</span>
                      <span class="tiny-text d-block text-muted" style="font-size: 0.65rem; font-weight: 800;">FAJLOVA</span>
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-6">
                <div class="ps-3 border-start">
                  <div v-for="(item, index) in subjectStats" :key="index" class="d-flex align-items-center mb-2">
                    <div class="rounded-circle me-2" :style="{ width: '10px', height: '10px', backgroundColor: colors[index % colors.length], flexShrink: '0' }"></div>
                    <div class="d-flex flex-column min-width-0 w-100">
                      <span class="fw-bold text-dark text-truncate" style="font-size: 0.8rem;">{{ item.label }}</span>
                      <span class="text-muted fw-semibold" style="font-size: 0.7rem;">{{ Math.round(item.percentage) }}%</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100">
          <h5 class="fw-bold mb-4 text-dark"><i class="bi bi-file-earmark-check-fill me-2 text-success"></i>Tipovi materijala</h5>
          <div v-for="stat in materialStats" :key="stat.label" class="material-stat mb-3">
            <div class="d-flex align-items-center gap-3">
              <div class="icon-circle shadow-sm" :class="getTheme(stat.label).bg">
                <i class="bi fs-5" :class="getTheme(stat.label).icon"></i>
              </div>
              <div class="w-100">
                <div class="d-flex justify-content-between mb-1">
                  <span class="fw-bold small text-dark">{{ stat.label }}</span>
                  <span class="text-muted tiny-label">{{ Math.round(stat.percentage) }}%</span>
                </div>
                <div class="progress rounded-pill" style="height: 6px;">
                  <div class="progress-bar" :class="getTheme(stat.label).bar" :style="{ width: stat.percentage + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="row">
      <div class="col-12">
        <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
          <div class="card-header bg-white border-0 py-4 px-4">
            <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
              <h5 class="fw-bold mb-0 text-dark">Moji materijali</h5>
              <div class="input-group input-group-sm border rounded-pill bg-light px-2" style="width: 280px;">
                <span class="input-group-text bg-transparent border-0"><i class="bi bi-search text-muted"></i></span>
                <input v-model="profileParams.searchTerm" @input="debounceProfileSearch" type="text" class="form-control bg-transparent border-0 shadow-none py-2" placeholder="Pretraži fajlove...">
              </div>
            </div>
          </div>

          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover align-middle mb-0">
                <thead class="bg-light text-uppercase small fw-bold text-muted">
                  <tr>
                    <th class="ps-4 py-3">Naziv materijala</th>
                    <th>Predmet</th>
                    <th>Datum objave</th>
                    <th class="text-end pe-4">Opcije</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="file in myMaterials" :key="file.materialId">
                    <td class="ps-4 py-3">
                      <div class="d-flex align-items-center">
                        <i :class="['bi fs-4 me-3', getFileIcon(file.type)]"></i>
                        <div>
                          <span class="fw-semibold text-dark d-block">{{ file.name }}</span>
                          <small class="text-muted">{{ file.authorName }}</small>
                        </div>
                      </div>
                    </td>
                    <td>
                      <span class="badge bg-primary-subtle text-primary rounded-pill px-3 fw-medium border border-primary-subtle">
                        {{ file.subjectName || file.nameSubject }}
                      </span>
                    </td>
                    <td class="text-muted small">{{ formatDate(file.dateCreated) }}</td>
                    <td class="text-end pe-4">
                      <div class="btn-group">
                        <button @click="downloadFile(file.materialId, file.name)" class="btn btn-sm btn-outline-primary rounded-pill px-3 me-2 border-primary-subtle">
                          <i class="bi bi-download"></i>
                        </button>
                        <button @click="openEditModal(file)" class="btn btn-sm btn-outline-warning rounded-pill px-3 me-2 border-warning-subtle">
                          <i class="bi bi-pencil-square"></i>
                        </button>
                        <button @click="openDeleteModal(file)" class="btn btn-sm btn-outline-danger rounded-pill px-3 border-danger-subtle">
                          <i class="bi bi-trash3"></i>
                        </button>
                      </div>
                    </td>
                  </tr>
                  <tr v-if="myMaterials.length === 0">
                    <td colspan="4" class="text-center py-5 text-muted">Nema pronađenih materijala.</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <div class="d-flex justify-content-between align-items-center p-4 border-top bg-light-subtle">
              <span class="small text-muted">Stranica <b>{{ profileParams.page }}</b></span>
              <div class="pagination-btns">
                <button @click="changeProfilePage(-1)" :disabled="profileParams.page === 1" class="btn btn-sm btn-white border shadow-sm px-3 rounded-pill me-2">Prethodna</button>
                <button @click="changeProfilePage(1)" :disabled="myMaterials.length < profileParams.pageSize" class="btn btn-sm btn-white border shadow-sm px-3 rounded-pill">Sledeća</button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="editModal.show" class="custom-overlay">
      <div class="custom-modal shadow-lg">
        <div class="icon-circle bg-warning-subtle">
          <i class="bi bi-pencil-fill text-warning"></i>
        </div>
        <h3 class="fw-bold text-dark mb-2">Izmeni naziv</h3>
        <p class="text-muted mb-4 px-4">Promenite naziv materijala kako bi bio jasniji kolegama.</p>
        <div class="px-4 mb-4">
          <input v-model="editModal.newName" type="text" class="form-control rounded-pill px-4 py-2 shadow-none border-light-subtle bg-light fw-bold text-center">
        </div>
        <div class="d-flex gap-3 justify-content-center px-4 mb-2">
          <button @click="editModal.show = false" class="btn btn-light px-4 py-2 rounded-pill fw-bold border w-50">Otkaži</button>
          <button @click="confirmEdit" class="btn btn-warning px-4 py-2 rounded-pill fw-bold shadow-sm w-50 text-white">Sačuvaj</button>
        </div>
      </div>
    </div>

    <div v-if="deleteModal.show" class="custom-overlay">
      <div class="custom-modal shadow-lg border-danger-subtle border">
        <div class="icon-circle bg-danger-subtle">
          <i class="bi bi-exclamation-triangle-fill text-danger"></i>
        </div>
        <h3 class="fw-bold text-dark mb-2">Brisanje materijala</h3>
        <p class="text-muted mb-4 px-4">
          Da li ste sigurni da želite da obrišete <span class="fw-bold text-dark">{{ deleteModal.fileName }}</span>? <br>
          Ova akcija se ne može poništiti.
        </p>
        <div class="d-flex gap-3 justify-content-center px-4 mb-2">
          <button @click="deleteModal.show = false" class="btn btn-light px-4 py-2 rounded-pill fw-bold border w-50">Otkaži</button>
          <button @click="confirmDelete" class="btn btn-danger px-4 py-2 rounded-pill fw-bold shadow-sm w-50">Izbriši</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';

// State
const materialStats = ref<any[]>([]);
const subjectStats = ref<any[]>([]);
const totalMaterials = ref(0);
const loading = ref(true);
const colors = ['#0d6efd', '#198754', '#ffc107', '#dc3545', '#6610f2', '#fd7e14', '#0dcaf0'];

const myMaterials = ref<any[]>([]);
const profileParams = ref({
  userId: null as number | null,
  searchTerm: '',
  page: 1,
  pageSize: 10,
  sortBy: 'date'
});

const currentUser = ref({
  firstName: '', 
  lastName: '', 
  email: '', 
  role: '', 
  departmentName: '', 
  userId: null as number | null
});

// Modal states
const deleteModal = ref({ show: false, materialId: 0, fileName: '' });
const editModal = ref({ show: false, materialId: 0, newName: '' });

// Fetching
const fetchData = async () => {
  try {
    loading.value = true;
    const userRes = await axios.get('http://localhost:5145/api/Auth/me', { withCredentials: true });
    if (userRes.data) {
      currentUser.value = userRes.data;
      profileParams.value.userId = userRes.data.userId;
      
      const statsRes = await axios.get(`http://localhost:5145/api/Material/dashboard-stats/${userRes.data.userId}`, { 
        params: { departmentName: userRes.data.departmentName },
        withCredentials: true 
      });
      materialStats.value = statsRes.data.types || [];
      subjectStats.value = statsRes.data.subjects || [];
      totalMaterials.value = statsRes.data.totalCount || 0;
      
      fetchMyMaterials();
    }
  } catch (e) { 
    console.error(e); 
  } finally { 
    loading.value = false; 
  }
};

const fetchMyMaterials = async () => {
  try {
    const res = await axios.get('http://localhost:5145/api/Material', { params: profileParams.value, withCredentials: true });
    myMaterials.value = res.data;
  } catch (e) { console.error(e); }
};

// Actions
const openDeleteModal = (file: any) => {
  deleteModal.value = { show: true, materialId: file.materialId, fileName: file.name };
};

const confirmDelete = async () => {
  try {
    await axios.delete(`http://localhost:5145/api/Material/${deleteModal.value.materialId}`, { withCredentials: true });
    deleteModal.value.show = false;
    fetchData(); // Refresh list and stats
  } catch (e) { alert("Greška pri brisanju."); }
};

const openEditModal = (file: any) => {
  editModal.value = { show: true, materialId: file.materialId, newName: file.name };
};

const confirmEdit = async () => {
  try {
    await axios.put(`http://localhost:5145/api/Material/${editModal.value.materialId}`, { 
      name: editModal.value.newName 
    }, { withCredentials: true });
    editModal.value.show = false;
    fetchMyMaterials();
  } catch (e) { alert("Greška pri izmeni."); }
};

const downloadFile = async (id: number, name: string) => {
  const res = await axios.get(`http://localhost:5145/api/Material/download/${id}`, { responseType: 'blob', withCredentials: true });
  const url = window.URL.createObjectURL(new Blob([res.data]));
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', name);
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
};

// UI Helpers
const donutGradient = computed(() => {
  if (!subjectStats.value.length) return 'background: #f0f0f0';
  let pos = 0;
  const segs = subjectStats.value.map((s, i) => {
    const start = pos; pos += s.percentage;
    return `${colors[i % colors.length]} ${start}% ${pos}%`;
  });
  return `background: conic-gradient(${segs.join(', ')})`;
});

const getTheme = (t: string) => {
  const themes: any = { 
    'Pdf': { icon: 'bi-file-earmark-pdf text-danger', bg: 'bg-light-danger', bar: 'bg-danger' }, 
    'Image': { icon: 'bi-image text-warning', bg: 'bg-light-warning', bar: 'bg-warning' }, 
    'Document': { icon: 'bi-file-earmark-word text-primary', bg: 'bg-light-primary', bar: 'bg-primary' }, 
    'Video': { icon: 'bi-play-circle text-success', bg: 'bg-light-success', bar: 'bg-success' } 
  };
  return themes[t] || { icon: 'bi-file-earmark', bg: 'bg-light', bar: 'bg-secondary' };
};

const getFileIcon = (type: any) => {
  const icons: any = { 
    1: 'bi-file-earmark-pdf text-danger', 'Pdf': 'bi-file-earmark-pdf text-danger', 
    2: 'bi-play-circle text-success', 'Video': 'bi-play-circle text-success', 
    3: 'bi-image text-warning', 'Image': 'bi-image text-warning', 
    4: 'bi-file-earmark-word text-primary', 'Document': 'bi-file-earmark-word text-primary' 
  };
  return icons[type] || 'bi-file-earmark text-secondary';
};

const formatDate = (d: string) => new Date(d).toLocaleDateString('sr-RS');

let timeout: any;
const debounceProfileSearch = () => { 
  clearTimeout(timeout); 
  timeout = setTimeout(() => { 
    profileParams.value.page = 1; 
    fetchMyMaterials(); 
  }, 500); 
};

const changeProfilePage = (s: number) => { 
  profileParams.value.page += s; 
  fetchMyMaterials(); 
};

onMounted(fetchData);
</script>

<style scoped>
.avatar-container { position: relative; width: 100px; height: 100px; }
.avatar-box { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 4px solid #fff; }
.tiny-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; color: #999; }
.info-list { background-color: #fcfcfc; padding: 15px; border-radius: 15px; border: 1px solid #f0f0f0; }
.donut-container { width: 140px; height: 140px; }
.donut-chart { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.donut-center { width: 75%; height: 75%; background: white; border-radius: 50%; display: flex; flex-direction: column; align-items: center; justify-content: center; box-shadow: inset 0 0 10px rgba(0,0,0,0.05); }
.icon-circle { width: 45px; height: 45px; border-radius: 12px; display: flex; align-items: center; justify-content: center; margin: 0 auto 1rem; }
.bg-light-danger { background: #fff5f5; }
.bg-light-warning { background: #fffcf0; }
.bg-light-success { background: #f0fff4; }
.bg-light-primary { background: #f0f7ff; }
.btn-white { background: white; transition: all 0.2s; }
.btn-white:hover { background: #f8f9fa; }
.activity-chart { max-height: 280px; overflow-y: auto; scrollbar-width: thin; }

/* Modal Styles */
.custom-overlay {
  position: fixed;
  inset: 0;
  background: rgba(13, 110, 253, 0.15);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}

.custom-modal {
  background: white;
  padding: 2rem 1.2rem;
  border-radius: 30px;
  width: 90%;
  max-width: 400px;
  text-align: center;
  animation: modalAppear 0.3s ease-out;
}

@keyframes modalAppear {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
</style>