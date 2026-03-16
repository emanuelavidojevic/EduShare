<template>
  <div class="subjects-page">
    <div class="row align-items-end mb-5">
      <div class="col-md-8">
        <h6 class="text-primary fw-bold text-uppercase tracking-wider mb-2" style="font-size: 0.75rem; letter-spacing: 1.5px;">Akademski pregled</h6>
        <h2 class="display-6 fw-bold text-dark mb-0">Tvoja baza znanja 📚</h2>
        <p class="text-muted mb-0 mt-2">
          Pristupi materijalima za <span class="fw-bold text-dark">{{ subjects.length }} predmeta</span> na tvom trenutnom smeru.
        </p>
      </div>
      
      <div v-if="['Admin', 'Profesor'].includes(authStore.user?.role)" class="col-md-4 text-md-end mt-3 mt-md-0">
        <button class="btn btn-primary shadow-sm rounded-pill px-4 py-2 fw-semibold transition-btn" @click="showAddModal = true">
          <i class="bi bi-plus-lg me-2"></i>Dodaj novi predmet
        </button>
      </div>
    </div>

    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-grow text-primary" role="status"></div>
      <p class="mt-3 text-muted fw-medium">Sređujemo tvoje predmete...</p>
    </div>

    <div v-else class="row g-4">
      <div v-for="subject in subjects" :key="subject.subjectId" class="col-md-6 col-lg-4">
        <div class="card h-100 subject-card shadow-sm border-0 position-relative overflow-hidden" @click="goToSubject(subject.subjectId)">
          <div class="card-glow"></div>
          <div class="card-body text-center p-4 position-relative z-index-2">
            
            <div :class="['icon-box mb-4 shadow-sm', getDesign(subject.nameSubject).bg]">
              <i :class="['bi', getDesign(subject.nameSubject).icon, getDesign(subject.nameSubject).text, 'fs-1']"></i>
            </div>
            
            <h5 class="fw-bold text-dark mb-1">{{ subject.nameSubject }}</h5>
            
            <div class="mb-3">
              <span class="professor-label">
                <i class="bi bi-person-badge me-1"></i>
                {{ subject.professorName || 'prof. nije dodeljen' }}
              </span>
            </div>

            <p class="text-secondary small mb-4 px-3 line-clamp-2" style="min-height: 40px;">
              {{ subject.descriptionSubject || 'Nema opisa za ovaj predmet.' }}
            </p>
            
            <div class="d-flex justify-content-center align-items-center">
              <span class="modern-subject-tag">
                <i class="bi bi-layers-half me-2"></i>
                <span class="fw-bold">{{ subject.materialCount }}</span> materijala
              </span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showAddModal" class="modal-backdrop fade show"></div>
    <div v-if="showAddModal" class="modal fade show d-block" tabindex="-1">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg rounded-4">
          <div class="modal-header border-0 p-4 pb-0">
            <h5 class="fw-bold text-dark mb-0">Novi Predmet</h5>
            <button type="button" class="btn-close" @click="showAddModal = false"></button>
          </div>
          <div class="modal-body p-4">
            <div class="mb-3">
              <label class="form-label small fw-bold text-muted text-uppercase">Naziv predmeta</label>
              <input v-model="newSubject.nameSubject" type="text" class="form-control rounded-3" placeholder="npr. Programiranje C">
            </div>
            <div class="mb-3">
              <label class="form-label small fw-bold text-muted text-uppercase">Opis</label>
              <textarea v-model="newSubject.descriptionSubject" class="form-control rounded-3" rows="3" placeholder="Kratak opis predmeta..."></textarea>
            </div>
          </div>
          <div class="modal-footer border-0 p-4 pt-0">
            <button type="button" class="btn btn-light rounded-pill px-4" @click="showAddModal = false">Odustani</button>
            <button type="button" class="btn btn-primary rounded-pill px-4 shadow-sm" @click="addSubject" :disabled="isSubmitting">
              <span v-if="isSubmitting" class="spinner-border spinner-border-sm me-2"></span>
              Sačuvaj predmet
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth.js';
import api from '../service/api';

const router = useRouter();
const authStore = useAuthStore();
const subjects = ref([]);
const isLoading = ref(true);
const showAddModal = ref(false);
const isSubmitting = ref(false);

const newSubject = ref({ nameSubject: '', descriptionSubject: '' });

const fetchSubjects = async () => {
  isLoading.value = true;
  try {
    const res = await api.get('Subject/my-subjects');
    subjects.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    isLoading.value = false;
  }
};

const addSubject = async () => {
  if (!newSubject.value.nameSubject) return;
  isSubmitting.value = true;
  try {
    await api.post('Subject', newSubject.value);
    showAddModal.value = false;
    newSubject.value = { nameSubject: '', descriptionSubject: '' };
    await fetchSubjects();
  } catch (error) {
    console.error(error);
  } finally {
    isSubmitting.value = false;
  }
};

const goToSubject = (id) => {
  router.push(`/dashboard/subject/${id}`);
};

const getDesign = (name) => {
  const n = name ? name.toLowerCase().trim() : '';
  
  // --- SRT DIZAJN ---
  if (n === 'c' || n === 'programiranje c') return { icon: 'bi-terminal-fill', bg: 'bg-c-light', text: 'text-c' };
  if (n.includes('klijent server')) return { icon: 'bi-cpu-fill', bg: 'bg-ks-light', text: 'text-ks' };
  if (n.includes('c#')) return { icon: 'bi-hash', bg: 'bg-purple-light', text: 'text-purple' };
  if (n.includes('baze')) return { icon: 'bi-database-fill', bg: 'bg-orange-light', text: 'text-orange' };
  if (n.includes('internet')) return { icon: 'bi-globe', bg: 'bg-green-light', text: 'text-success' };
  if (n.includes('python')) return { icon: 'bi-code-slash', bg: 'bg-blue-light', text: 'text-primary' };

  // --- KOT & MREŽE DIZAJN ---
  if (n.includes('telekomunikacije') || n.includes('tk')) return { icon: 'bi-reception-4', bg: 'bg-indigo-light', text: 'text-indigo' };
  if (n.includes('mreze') || n.includes('mrežni')) return { icon: 'bi-diagram-3-fill', bg: 'bg-blue-light', text: 'text-primary' };
  if (n.includes('elektronika')) return { icon: 'bi-lightning-charge-fill', bg: 'bg-yellow-light', text: 'text-warning' };
  if (n.includes('projektovanje') || n.includes('cad')) return { icon: 'bi-layers-half', bg: 'bg-teal-light', text: 'text-teal' };
  if (n.includes('servisi')) return { icon: 'bi-server', bg: 'bg-ks-light', text: 'text-ks' };
  if (n.includes('merenja') || n.includes('instrumentacija')) return { icon: 'bi-speedometer2', bg: 'bg-cyan-light', text: 'text-cyan' };
  if (n.includes('mikro')) return { icon: 'bi-memory', bg: 'bg-red-light', text: 'text-danger' };
  if (n.includes('robotika')) return { icon: 'bi-robot', bg: 'bg-indigo-light', text: 'text-indigo' };

  return { icon: 'bi-journal-code', bg: 'bg-light', text: 'text-secondary' };
};

onMounted(fetchSubjects);
</script>

<style scoped>
.subjects-page { animation: fadeIn 0.5s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }
.subject-card { transition: all 0.4s cubic-bezier(0.165, 0.84, 0.44, 1); cursor: pointer; border-radius: 24px; background: #ffffff; border: 1px solid #f1f5f9 !important; }
.subject-card:hover { transform: translateY(-12px); box-shadow: 0 20px 40px rgba(15, 23, 42, 0.08) !important; }
.card-glow { position: absolute; top: -50%; left: -50%; width: 200%; height: 200%; background: radial-gradient(circle, rgba(13, 110, 253, 0.03) 0%, rgba(255, 255, 255, 0) 50%); opacity: 0; transition: opacity 0.4s; }
.subject-card:hover .card-glow { opacity: 1; }
.icon-box { width: 80px; height: 80px; margin: 0 auto; display: flex; align-items: center; justify-content: center; border-radius: 20px; transition: transform 0.4s ease; }
.subject-card:hover .icon-box { transform: scale(1.1) rotate(5deg); }
.professor-label { font-size: 0.82rem; color: #94a3b8; font-weight: 500; display: inline-flex; align-items: center; transition: color 0.3s ease; }
.subject-card:hover .professor-label { color: #64748b; }

/* SRT Boje */
.bg-purple-light { background-color: #f3e5f5 !important; } 
.text-purple { color: #9c27b0 !important; }
.bg-blue-light { background-color: #e3f2fd !important; } 
.text-primary { color: #0d6efd !important; }
.bg-orange-light { background-color: #fff3e0 !important; } 
.text-orange { color: #ff9800 !important; }
.bg-green-light { background-color: #e8f5e9 !important; }
.text-success { color: #198754 !important; }
.bg-c-light { background-color: #f8f9fa !important; border: 1px solid #dee2e6; }
.text-c { color: #495057 !important; }
.bg-ks-light { background-color: #e0f7fa !important; }
.text-ks { color: #00acc1 !important; }

/* KOT & Mreže Boje */
.bg-indigo-light { background-color: #e8eaf6 !important; }
.text-indigo { color: #3f51b5 !important; }
.bg-teal-light { background-color: #e0f2f1 !important; }
.text-teal { color: #009688 !important; }
.bg-cyan-light { background-color: #e0f7fa !important; }
.text-cyan { color: #00bcd4 !important; }
.bg-yellow-light { background-color: #fffde7 !important; }
.text-warning { color: #fbc02d !important; }
.bg-red-light { background-color: #ffebee !important; }
.text-danger { color: #ef5350 !important; }
.bg-grey-light { background-color: #f5f5f5 !important; }

.modern-subject-tag { background: #f8fafc; color: #475569; padding: 8px 18px; border-radius: 14px; font-size: 0.85rem; border: 1px solid #f1f5f9; }
.modal-backdrop { background-color: rgba(15, 23, 42, 0.5); backdrop-filter: blur(4px); }
.line-clamp-2 { display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
.transition-btn { transition: all 0.3s ease; }
.transition-btn:hover { transform: scale(1.05); }
</style>