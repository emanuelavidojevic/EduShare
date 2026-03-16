<template>
  <div class="subject-detail p-4 bg-light min-vh-100">
    <div class="d-flex align-items-center mb-4">
      <button @click="router.push('/dashboard/subject')" class="btn btn-white border shadow-sm rounded-circle me-3 d-flex align-items-center justify-content-center" style="width: 40px; height: 40px;">
        <i class="bi bi-arrow-left fs-5"></i>
      </button>
      <div>
        <h2 class="fw-bold mb-0 text-dark">
          {{ currentSubjectName || 'Učitavanje...' }} 
          <span v-if="professorName" class="text-secondary fw-medium ms-1" style="font-size: 1.2rem;"> — {{ professorName }}</span>
        </h2>
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-0 small text-muted">
            <li class="breadcrumb-item"><router-link to="/dashboard/subject" class="text-decoration-none text-muted">Predmeti</router-link></li>
            <li class="breadcrumb-item active text-primary fw-bold">{{ currentSubjectName || 'Predmet' }}</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="card border-0 shadow-sm rounded-4 overflow-hidden bg-white">
      <div class="card-header bg-white border-0 py-4 px-4">
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
          <h5 class="fw-bold mb-0 text-dark">Materijali za ovaj predmet</h5>
          <div class="d-flex gap-3 align-items-center">
            <div class="input-group input-group-sm border rounded-pill bg-light px-2" style="width: 280px;">
              <span class="input-group-text bg-transparent border-0"><i class="bi bi-search text-muted"></i></span>
              <input v-model="queryParams.searchTerm" @input="debounceSearch" type="text" class="form-control bg-transparent border-0 shadow-none py-2" placeholder="Pretraži fajlove...">
            </div>
            
            <button v-if="canAdd" @click="showModal = true" class="btn btn-primary rounded-pill px-4 shadow-sm fw-bold d-flex align-items-center">
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
                <th class="ps-4 py-3 border-0">Naziv materijala</th>
                <th class="border-0 text-center">Ocena</th>
                <th class="border-0">Autor</th>
                <th class="border-0">Datum objave</th>
                <th class="border-0 text-end pe-4">Opcije</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="file in materials" :key="file.materialId">
                <td class="ps-4 py-3" @click="openRatingModal(file)" style="cursor: pointer;">
                  <div class="d-flex align-items-center">
                    <i :class="['bi fs-4 me-3', getFileIcon(file.type, file.name)]"></i>
                    <span class="fw-semibold text-dark">{{ file.name }}</span>
                  </div>
                </td>
                <td class="text-center">
                  <div class="rating-display text-warning small" @click="openRatingModal(file)" style="cursor: pointer;">
                    <i v-for="i in 5" :key="i" :class="['bi', i <= Math.round(file.averageRating || 0) ? 'bi-star-fill' : 'bi-star']"></i>
                    <span class="ms-1 text-muted">({{ file.averageRating || 0 }})</span>
                  </div>
                </td>
                <td>
                  <div class="d-flex align-items-center">
                    <div class="avatar-sm me-2 bg-primary-subtle text-primary rounded-circle d-flex align-items-center justify-content-center fw-bold small" style="width: 28px; height: 28px; font-size: 10px;">
                      {{ file.authorName?.split(' ').map(n => n[0]).join('').toUpperCase() || '?' }}
                    </div>
                    <span class="small fw-medium text-muted">{{ file.authorName }}</span>
                  </div>
                </td>
                <td class="text-muted small">{{ formatDate(file.dateCreated) }}</td>
                <td class="text-end pe-4">
                  <div class="btn-group">
                    <button @click="downloadFile(file.materialId, file.name)" class="btn btn-sm btn-outline-primary rounded-pill px-3 me-2 border-primary-subtle fw-medium">
                      <i class="bi bi-download"></i>
                    </button>
                    <template v-if="canEditOrDelete(file)">
                      <button @click="openEditModal(file)" class="btn btn-sm btn-outline-warning rounded-pill px-3 me-2 border-warning-subtle fw-medium">
                        <i class="bi bi-pencil-square"></i>
                      </button>
                      <button @click="openDeleteModal(file)" class="btn btn-sm btn-outline-danger rounded-pill px-3 border-danger-subtle fw-medium">
                        <i class="bi bi-trash3"></i>
                      </button>
                    </template>
                  </div>
                </td>
              </tr>
              <tr v-if="materials.length === 0">
                <td colspan="5" class="text-center py-5 text-muted">Nema materijala.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="custom-overlay">
      <div class="custom-modal shadow-lg">
        <div class="icon-circle bg-primary-subtle">
          <i class="bi bi-cloud-arrow-up-fill text-primary"></i>
        </div>
        <h3 class="fw-bold text-dark mb-2">Novi materijal</h3>
        <p class="text-muted mb-4 px-3">Izaberite fajl i dajte mu prepoznatljiv naziv.</p>
        <form @submit.prevent="handleUpload" class="px-4">
          <div class="mb-3 text-start">
            <label class="form-label small fw-bold text-muted text-uppercase ms-2">Naziv</label>
            <input v-model="uploadForm.name" type="text" class="form-control rounded-pill px-3 shadow-none border-light-subtle bg-light" required>
          </div>
          <div class="mb-4 text-start">
            <label class="form-label small fw-bold text-muted text-uppercase ms-2">Fajl</label>
            <input type="file" @change="onFileSelected" class="form-control rounded-pill px-3 shadow-none border-light-subtle bg-light" required>
          </div>
          <div class="d-flex gap-3 justify-content-center mb-2">
            <button type="button" @click="showModal = false" class="btn btn-light px-4 py-2 rounded-pill fw-bold border w-50">Otkaži</button>
            <button type="submit" class="btn btn-primary px-4 py-2 rounded-pill fw-bold shadow-sm w-50" :disabled="isUploading">
              <span v-if="isUploading" class="spinner-border spinner-border-sm me-1"></span> Potvrdi
            </button>
          </div>
        </form>
      </div>
    </div>

    <div v-if="activeMaterial" class="custom-overlay">
      <div class="custom-modal shadow-lg" style="max-width: 500px;">
        <div class="d-flex justify-content-end px-3 pt-3">
          <button @click="activeMaterial = null" class="btn-close shadow-none"></button>
        </div>
        <div class="px-4 pb-4">
          <div class="icon-circle bg-warning-subtle mb-3">
            <i class="bi bi-star-fill text-warning"></i>
          </div>
          <h4 class="fw-bold text-dark mb-1">Ocena i komentari</h4>
          <p class="text-muted small mb-4">Ocenite materijal i pogledajte iskustva drugih.</p>

          <div class="rating-box p-3 bg-light rounded-4 mb-4 text-center border border-white">
            <p class="small fw-bold text-muted text-uppercase mb-2">Tvoja ocena</p>
            <div class="fs-1 text-warning">
              <i v-for="i in 5" :key="i" 
                 :class="['bi mx-1 star-hover', i <= (userRating || activeMaterial.userRating) ? 'bi-star-fill' : 'bi-star']" 
                 @click="submitRating(i)" style="cursor: pointer;"></i>
            </div>
          </div>

          <div class="comments-container text-start">
            <h6 class="fw-bold mb-3 d-flex align-items-center">
              <i class="bi bi-chat-left-text me-2 text-primary"></i> Komentari
            </h6>
            <div class="comment-scroll mb-3 px-2" style="max-height: 250px; overflow-y: auto;">
              <div v-for="(c, index) in materialComments" :key="c.commentId || index" 
                   class="mb-3 p-3 bg-white rounded-4 border-start border-primary border-4 shadow-sm">
                <div class="d-flex justify-content-between align-items-center mb-1">
                  <span class="fw-bold text-dark small">
                    {{ c.user?.firstName || 'Korisnik' }} {{ c.user?.lastName || '' }}
                  </span>
                  <span class="text-muted" style="font-size: 0.7rem;">
                    {{ formatDate(c.dateComments) }}
                  </span>
                </div>
                <p class="mb-0 text-muted small" style="line-height: 1.4;">
                  {{ c.textOfComments }}
                </p>
              </div>
              <div v-if="!materialComments || materialComments.length === 0" class="text-center py-4 text-muted small italic">
                <i class="bi bi-chat-dots d-block fs-2 mb-2 opacity-25"></i>
                Budi prvi koji će prokomentarisati.
              </div>
            </div>
            <div class="comment-input-group d-flex gap-2">
              <input v-model="newCommentText" type="text" 
                     class="form-control rounded-pill border-light-subtle px-3 shadow-none bg-light" 
                     placeholder="Dodaj komentar..." @keyup.enter="submitComment">
              <button @click="submitComment" class="btn btn-primary rounded-circle d-flex align-items-center justify-content-center" style="width: 45px; height: 45px;">
                <i class="bi bi-send-fill"></i>
              </button>
            </div>
          </div>
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth.js';
import api from '../service/api';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

// --- STATE ---
const materials = ref<any[]>([]);
const currentSubjectName = ref('');
const professorName = ref('');
const showModal = ref(false);
const isUploading = ref(false);
const selectedFile = ref<File | null>(null);
const activeMaterial = ref<any>(null);
const materialComments = ref<any[]>([]);
const newCommentText = ref('');
const userRating = ref(0);

const deleteModal = ref({ show: false, materialId: 0, fileName: '' });
const editModal = ref({ show: false, materialId: 0, newName: '' });

const queryParams = ref({
  subjectId: Number(route.params.id),
  searchTerm: '',
  page: 1,
  pageSize: 10
});

const uploadForm = ref({ name: '', description: '' });

// --- DOZVOLE ---
const canAdd = computed(() => {
  const user = authStore.user;
  if (!user || !user.role) return false;
  const role = user.role.toLowerCase();
  if (role === 'admin' || role === 'student') return true;
  if (role === 'professor' || role === 'profesor') {
    return `${user.firstName} ${user.lastName}`.trim().toLowerCase() === professorName.value.trim().toLowerCase();
  }
  return false;
});

const canEditOrDelete = (file: any) => {
  const user = authStore.user;
  if (!user || !user.role) return false;
  const role = user.role.toLowerCase();
  const currentUserId = Number(user.userId || user.UserId || user.id);
  const fileUserId = Number(file.userId || file.UserId);
  if (role === 'admin') return true;
  if (role === 'professor' || role === 'profesor') {
    if (`${user.firstName} ${user.lastName}`.trim().toLowerCase() === professorName.value.trim().toLowerCase()) return true;
  }
  return (role === 'student' && currentUserId === fileUserId);
};

// --- LOGIKA ---
const fetchMaterials = async () => {
  try {
    const res = await api.get('Material', { params: queryParams.value });
    materials.value = res.data;
    if (res.data && res.data.length > 0) {
      currentSubjectName.value = res.data[0].nameSubject;
      professorName.value = res.data[0].professorName || '';
    }
  } catch (e) { console.error(e); }
};

const handleUpload = async () => {
  if (!selectedFile.value || !authStore.user) return;
  isUploading.value = true;
  try {
    const user = authStore.user;
    const formData = new FormData();
    formData.append('File', selectedFile.value);
    formData.append('Name', uploadForm.value.name);
    formData.append('SubjectId', route.params.id.toString());
    formData.append('UserId', (user.userId || user.UserId || user.id).toString());

    await axios.post('http://localhost:5145/api/Material/upload', formData, { 
      withCredentials: true,
      headers: { 'Content-Type': 'multipart/form-data' }
    });
    showModal.value = false;
    selectedFile.value = null;
    uploadForm.value.name = '';
    fetchMaterials();
  } catch (e) { alert("Greška pri uploadu!"); }
  finally { isUploading.value = false; }
};

const openRatingModal = async (material: any) => {
  activeMaterial.value = material;
  materialComments.value = []; 
  userRating.value = 0;
  newCommentText.value = '';
  
  try {
    const res = await api.get(`Comment/material/${material.materialId}`);
    materialComments.value = Array.isArray(res.data) ? res.data : [];
  } catch (e) { console.error("Greška pri učitavanju komentara:", e); }
};

const submitComment = async () => {
  if (!newCommentText.value.trim() || !activeMaterial.value) return;
  try {
    await api.post(`Comment/${activeMaterial.value.materialId}`, { 
      textOfComments: newCommentText.value 
    });
    newCommentText.value = '';
    // Ponovo učitavamo da bismo dobili ime autora od backend-a
    const res = await api.get(`Comment/material/${activeMaterial.value.materialId}`);
    materialComments.value = res.data;
  } catch (e) { console.error(e); }
};

const submitRating = async (val: number) => {
  userRating.value = val;
  try {
    await api.post(`Rating/${activeMaterial.value.materialId}`, { valueForRatings: val });
    fetchMaterials(); // Osveži tabelu zbog proseka
  } catch (e) { console.error(e); }
};

const downloadFile = async (id: number, name: string) => {
  try {
    const res = await api.get(`Material/download/${id}`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([res.data]));
    const link = document.createElement('a');
    link.href = url; 
    link.setAttribute('download', name);
    document.body.appendChild(link); 
    link.click();
    document.body.removeChild(link);
  } catch (e) { console.error(e); }
};

const openDeleteModal = (file: any) => {
  deleteModal.value = { show: true, materialId: file.materialId, fileName: file.name };
};

const confirmDelete = async () => {
  try {
    await api.delete(`Material/${deleteModal.value.materialId}`);
    deleteModal.value.show = false;
    fetchMaterials();
  } catch (e) { console.error(e); }
};

const openEditModal = (file: any) => {
  editModal.value = { show: true, materialId: file.materialId, newName: file.name };
};

const confirmEdit = async () => {
  try {
    await api.put(`Material/${editModal.value.materialId}`, { name: editModal.value.newName });
    editModal.value.show = false;
    fetchMaterials();
  } catch (e) { console.error(e); }
};

const onFileSelected = (e: any) => {
  const file = e.target.files[0];
  if (file) {
    selectedFile.value = file;
    uploadForm.value.name = file.name.split('.')[0];
  }
};

const getFileIcon = (type: number, fileName = "") => {
  const name = fileName.toLowerCase();
  if (name.endsWith('.c') || name.endsWith('.h') || name.endsWith('.sql')) return 'bi-terminal text-primary';
  const icons: Record<number, string> = { 1: 'bi-file-earmark-pdf text-danger', 2: 'bi-play-circle text-success', 3: 'bi-image text-warning' };
  return icons[type] || 'bi-file-earmark text-secondary';
};

const formatDate = (d: any) => d ? new Date(d).toLocaleDateString('sr-RS') : '-';

let timeout: any;
const debounceSearch = () => {
  clearTimeout(timeout);
  timeout = setTimeout(() => { queryParams.value.page = 1; fetchMaterials(); }, 500);
};

onMounted(fetchMaterials);
</script>

<style scoped>
.btn-white { background: white; color: #444; }

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
  border-radius: 35px;
  width: 90%;
  max-width: 440px;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.4);
  animation: modalAppear 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.15);
}

.icon-circle {
  width: 70px;
  height: 70px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.2rem;
  font-size: 2.2rem;
}

.star-hover:hover {
  transform: scale(1.2);
  transition: 0.2s;
}

.comment-scroll::-webkit-scrollbar { width: 5px; }
.comment-scroll::-webkit-scrollbar-thumb { background: #e0e0e0; border-radius: 10px; }

@keyframes modalAppear {
  from { transform: scale(0.9); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
</style>