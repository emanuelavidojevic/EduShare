<template>
  <div class="subject-detail p-4">
    <div class="d-flex align-items-center mb-4">
      <button @click="router.push('/dashboard/subject')" class="btn btn-white border shadow-sm rounded-circle me-3">
        <i class="bi bi-arrow-left"></i>
      </button>
      <div>
        <h2 class="fw-bold mb-0 text-dark">{{ currentSubjectName }}</h2>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-0 small text-muted">
            <li class="breadcrumb-item"><router-link to="/dashboard/subject" class="text-decoration-none">Predmeti</router-link></li>
            <li class="breadcrumb-item active text-primary">{{ currentSubjectName }}</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
      <div class="card-header bg-white border-0 py-4 px-4">
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
          <h5 class="fw-bold mb-0 text-dark">Materijali za ovaj predmet</h5>
          
          <div class="d-flex gap-3 align-items-center">
            <div class="input-group input-group-sm border rounded-pill bg-light px-2" style="width: 280px;">
              <span class="input-group-text bg-transparent border-0"><i class="bi bi-search text-muted"></i></span>
              <input v-model="queryParams.searchTerm" @input="debounceSearch" type="text" class="form-control bg-transparent border-0 shadow-none py-2" placeholder="Pretraži fajlove...">
            </div>
            
            <button @click="showModal = true" class="btn btn-primary rounded-pill px-4 shadow-sm fw-bold">
              <i class="bi bi-plus-lg me-2"></i>Dodaj materijal
            </button>
          </div>
        </div>
      </div>

      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light text-uppercase small fw-bold text-muted">
              <tr>
                <th class="ps-4 py-3">Naziv materijala</th>
                <th>Autor</th>
                <th>Datum objave</th>
                <th class="text-end pe-4">Opcije</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="file in materials" :key="file.materialId">
                <td class="ps-4 py-3">
                  <div class="d-flex align-items-center">
                    <i :class="['bi fs-4 me-3', getFileIcon(file.type)]"></i>
                    <span class="fw-semibold text-dark">{{ file.name }}</span>
                  </div>
                </td>
                <td>
                  <div class="d-flex align-items-center">
                    <div class="avatar-sm me-2 bg-primary-subtle text-primary rounded-circle d-flex align-items-center justify-content-center fw-bold" style="width: 32px; height: 32px; font-size: 11px;">
                      {{ file.authorName?.split(' ').map(n => n[0]).join('') }}
                    </div>
                    <span class="small fw-medium">{{ file.authorName }}</span>
                  </div>
                </td>
                <td class="text-muted small">{{ formatDate(file.dateCreated) }}</td>
                <td class="text-end pe-4">
                  <button class="btn btn-sm btn-outline-primary rounded-pill px-3 me-2" @click="downloadFile(file.materialId, file.name)">
                    <i class="bi bi-download me-1"></i> Preuzmi
                  </button>
                </td>
              </tr>
              <tr v-if="materials.length === 0">
                <td colspan="4" class="text-center py-5 text-muted">Nema pronađenih materijala.</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="d-flex justify-content-between align-items-center p-4 border-top bg-light-subtle">
          <span class="small text-muted">Stranica <b>{{ queryParams.page }}</b></span>
          <div class="pagination-btns">
            <button @click="changePage(-1)" :disabled="queryParams.page === 1" class="btn btn-sm btn-white border shadow-sm px-3 rounded-pill me-2">Prethodna</button>
            <button @click="changePage(1)" :disabled="materials.length < queryParams.pageSize" class="btn btn-sm btn-white border shadow-sm px-3 rounded-pill">Sledeća</button>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="custom-modal-overlay">
      <div class="custom-modal-card shadow-lg p-4 border-0 rounded-4" style="max-width: 450px; background: #fff; margin: auto; position: relative; top: 10%;">
        <h4 class="fw-bold mb-4">Otpremi materijal</h4>
        <form @submit.prevent="handleUpload">
          <div class="mb-3">
            <label class="form-label small fw-bold">Naziv materijala</label>
            <input v-model="uploadForm.name" type="text" class="form-control rounded-3" required placeholder="Npr. Vežbe 1 - Osnove">
          </div>
          <div class="mb-3">
            <label class="form-label small fw-bold">Kratak opis (opciono)</label>
            <textarea v-model="uploadForm.description" class="form-control rounded-3" rows="2" placeholder="Dodatne info..."></textarea>
          </div>
          <div class="mb-4">
            <label class="form-label small fw-bold text-primary">Izaberi fajl</label>
            <input type="file" @change="onFileSelected" class="form-control border-primary border-dashed" required>
          </div>
          <div class="d-flex gap-2">
            <button type="button" @click="showModal = false" class="btn btn-light w-100 rounded-pill fw-bold">Otkaži</button>
            <button type="submit" class="btn btn-primary w-100 rounded-pill fw-bold" :disabled="isUploading">
               {{ isUploading ? 'Slanje...' : 'Potvrdi' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const materials = ref([]);
const currentSubjectName = ref('');
const showModal = ref(false);
const isUploading = ref(false);
const selectedFile = ref<File | null>(null);

const queryParams = ref({
  subjectId: Number(route.params.id),
  searchTerm: '',
  page: 1,
  pageSize: 10,
  sortBy: 'date'
});

const uploadForm = ref({ name: '', description: '' });

const fetchMaterials = async () => {
  try {
    const res = await axios.get('http://localhost:5145/api/Material', { params: queryParams.value, withCredentials: true });
    materials.value = res.data;
    if (res.data.length > 0) currentSubjectName.value = res.data[0].nameSubject;
  } catch (e) { console.error(e); }
};

const handleUpload = async () => {
  if (!selectedFile.value) return;
  isUploading.value = true;
  const user = JSON.parse(localStorage.getItem('user_display') || '{}');
  const formData = new FormData();
  formData.append('File', selectedFile.value);
  formData.append('Name', uploadForm.value.name);
  formData.append('Description', uploadForm.value.description);
  formData.append('SubjectId', route.params.id as string);
  formData.append('UserId', user.userId);

  try {
    await axios.post('http://localhost:5145/api/Material/upload', formData, { withCredentials: true });
    showModal.value = false;
    uploadForm.value = { name: '', description: '' };
    fetchMaterials();
  } catch (e) { alert("Greška!"); } finally { isUploading.value = false; }
};

const onFileSelected = (e: any) => {
  selectedFile.value = e.target.files[0];
  if (!uploadForm.value.name) uploadForm.value.name = selectedFile.value?.name.split('.')[0] || '';
};

const changePage = (step: number) => {
  queryParams.value.page += step;
  fetchMaterials();
};

const getFileIcon = (type: string) => {
  const icons: any = { Pdf: 'bi-file-earmark-pdf text-danger', Image: 'bi-image text-warning', Document: 'bi-file-earmark-word text-primary' };
  return icons[type] || 'bi-file-earmark text-secondary';
};

const downloadFile = async (id: number, name: string) => {
  const res = await axios.get(`http://localhost:5145/api/Material/download/${id}`, { responseType: 'blob', withCredentials: true });
  const url = window.URL.createObjectURL(new Blob([res.data]));
  const link = document.createElement('a');
  link.href = url;
  link.setAttribute('download', name);
  document.body.appendChild(link);
  link.click();
};

const formatDate = (d: string) => new Date(d).toLocaleDateString('sr-RS');
let timeout: any;
const debounceSearch = () => { clearTimeout(timeout); timeout = setTimeout(() => { queryParams.value.page = 1; fetchMaterials(); }, 500); };

onMounted(fetchMaterials);
</script>

<style scoped>
.custom-modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.4); backdrop-filter: blur(4px); z-index: 9999; display: flex; }
.btn-white { background: white; }
</style>