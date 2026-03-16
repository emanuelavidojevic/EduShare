<template>
  <div v-if="userId" class="container-fluid py-4 animate__animated animate__fadeIn">
    <div class="row g-4">
      <div class="col-lg-8">
        <div class="card border-0 shadow-lg rounded-4 overflow-hidden">
          <div class="card-header bg-dark d-flex justify-content-between align-items-center py-3 px-4">
            <div class="d-flex gap-2">
              <div class="dot red"></div>
              <div class="dot yellow"></div>
              <div class="dot green"></div>
            </div>
            <div class="text-white-50 small font-monospace text-uppercase" style="letter-spacing: 2px;">
              editor
            </div>
          </div>
          <div class="editor-wrapper">
            <textarea 
              v-model="newEntry.code" 
              class="dark-textarea" 
              placeholder="/* Nalepi ovde svoj kod... */" 
              spellcheck="false">
            </textarea>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-white mb-4">
          <label class="form-label fw-bold small text-muted text-uppercase mb-2">Naslov fajla</label>
          <input 
            v-model="newEntry.filename" 
            type="text" 
            class="form-control custom-input shadow-none" 
            placeholder="npr. MojaKlasa">
        </div>

        <div class="mb-4">
          <label class="form-label fw-bold small text-muted text-uppercase d-block mb-3">Izaberi predmet</label>
          
          <div v-if="subjects.length > 0" class="subject-grid-colored">
            <div 
              v-for="subject in subjects" 
              :key="subject.subjectId"
              class="subject-mini-card"
              :class="{ 'active': newEntry.subjectId === subject.subjectId }"
              @click="newEntry.subjectId = subject.subjectId"
            >
              <div class="icon-circle shadow-sm" :style="{ backgroundColor: getSubjectColor(subject.nameSubject) }">
                <i :class="['bi', getSubjectIcon(subject.nameSubject), 'fs-4']"></i>
              </div>
              <span class="subject-mini-name">{{ subject.nameSubject }}</span>
              
              <div class="active-badge" v-if="newEntry.subjectId === subject.subjectId">
                <i class="bi bi-check-lg"></i>
              </div>
            </div>
          </div>
          <div v-else class="text-center p-3">
              <div class="spinner-border spinner-border-sm text-primary"></div>
          </div>
        </div>

        <button 
          @click="publishCode"
          class="btn btn-publish-action w-100 rounded-4 fw-bold shadow-lg py-3" 
          :disabled="loading || !newEntry.subjectId || !newEntry.code"
        >
          <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
          <i v-else class="bi bi-cloud-arrow-up-fill me-2"></i>OBJAVI NA CODESHARE
        </button>

        <div v-if="successMessage" class="alert alert-success mt-3 rounded-4 border-0 shadow-sm animate__animated animate__fadeIn">
          <i class="bi bi-check-circle-fill me-2"></i> {{ successMessage }}
        </div>
      </div>
    </div>
  </div>
  
  <div v-else class="d-flex justify-content-center align-items-center" style="height: 80vh;">
    <div class="text-center">
      <div class="spinner-border text-primary mb-2"></div>
      <p class="text-muted">Inicijalizacija razvojnog okruženja...</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import api from '../service/api'; 
import { useAuthStore } from '../stores/auth.js';

const authStore = useAuthStore();
const loading = ref(false);
const subjects = ref([]);
const successMessage = ref('');

const newEntry = ref({
  filename: '', 
  code: '',     
  subjectId: null
});

const userId = computed(() => authStore.user?.userId || authStore.user?.UserId);

// Sinhronizovane ikonice sa Subjects.vue
const getSubjectIcon = (name) => {
  const n = name ? name.toLowerCase().trim() : '';
  
  // SRT
  if (n === 'c' || n === 'programiranje c') return 'bi-terminal-fill';
  if (n.includes('klijent server')) return 'bi-cpu-fill';
  if (n.includes('c#')) return 'bi-hash';
  if (n.includes('baze')) return 'bi-database-fill';
  if (n.includes('internet')) return 'bi-globe';
  if (n.includes('python')) return 'bi-code-slash';

  // KOT & MREŽE
  if (n.includes('telekomunikacije') || n.includes('tk')) return 'bi-reception-4';
  if (n.includes('mreze') || n.includes('mrežni')) return 'bi-diagram-3-fill';
  if (n.includes('elektronika')) return 'bi-lightning-charge-fill';
  if (n.includes('projektovanje') || n.includes('cad')) return 'bi-layers-half';
  if (n.includes('servisi')) return 'bi-server';
  if (n.includes('merenja')) return 'bi-speedometer2';
  if (n.includes('mikro')) return 'bi-memory';
  if (n.includes('robotika')) return 'bi-robot';

  return 'bi-journal-code';
};

// Sinhronizovane boje sa Subjects.vue
const getSubjectColor = (name) => {
  const n = name ? name.toLowerCase().trim() : '';
  
  if (n === 'c' || n === 'programiranje c') return '#495057';
  if (n.includes('klijent server')) return '#00acc1';
  if (n.includes('c#')) return '#9c27b0';
  if (n.includes('baze')) return '#ff9800';
  if (n.includes('internet')) return '#198754';
  if (n.includes('python')) return '#0d6efd';
  
  if (n.includes('telekomunikacije') || n.includes('tk')) return '#3f51b5';
  if (n.includes('mreze') || n.includes('mrežni')) return '#0d6efd';
  if (n.includes('elektronika')) return '#fbc02d';
  if (n.includes('projektovanje') || n.includes('cad')) return '#009688';
  if (n.includes('mikro')) return '#ef5350';
  
  return '#64748b';
};

const fetchData = async () => {
  try {
    const res = await api.get('Subject/my-subjects');
    subjects.value = res.data;
  } catch (e) { console.error(e); }
};

const publishCode = async () => {
  if (!newEntry.value.code || !newEntry.value.subjectId || !userId.value) return;
  try {
    loading.value = true;
    const cleanName = newEntry.value.filename.trim() || 'Snippet';
    const blob = new Blob([newEntry.value.code], { type: 'text/plain' });
    const file = new File([blob], `${cleanName}.cs`, { type: 'text/plain' });
    
    const formData = new FormData();
    formData.append('UserId', userId.value.toString());
    formData.append('SubjectId', newEntry.value.subjectId.toString());
    formData.append('Name', cleanName);
    formData.append('Description', "CodeShare Upload");
    formData.append('File', file);

    await api.post('Material/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });

    successMessage.value = "Kôd je uspešno objavljen na platformi!";
    newEntry.value.code = ''; newEntry.value.filename = ''; newEntry.value.subjectId = null;
    setTimeout(() => { successMessage.value = ''; }, 5000);
  } catch (e) { alert("Slanje nije uspelo."); } finally { loading.value = false; }
};

onMounted(fetchData);
</script>

<style scoped>
.dot { width: 10px; height: 10px; border-radius: 50%; }
.red { background: #ff5f56; } .yellow { background: #ffbd2e; } .green { background: #27c93f; }
.editor-wrapper { background: #1e1e1e; padding: 5px; }
.dark-textarea {
  width: 100%; height: 600px; background: #1e1e1e; color: #d4d4d4;
  border: none; padding: 30px; font-family: 'Fira Code', 'Courier New', monospace; 
  font-size: 1rem; line-height: 1.6; resize: none; outline: none;
}
.custom-input { background-color: #f8fafc; border: 2px solid #e2e8f0; border-radius: 12px; padding: 12px; }
.subject-grid-colored { display: grid; grid-template-columns: repeat(2, 1fr); gap: 12px; }
.subject-mini-card { 
  background: white; border: 1px solid #e2e8f0; border-radius: 16px; 
  padding: 15px 5px; display: flex; flex-direction: column; 
  align-items: center; cursor: pointer; transition: 0.3s; position: relative; min-height: 120px; 
}
.subject-mini-card.active { border-color: #0d6efd; background: #f0f7ff; transform: translateY(-2px); box-shadow: 0 5px 15px rgba(13, 110, 253, 0.1); }
.icon-circle { width: 48px; height: 48px; border-radius: 12px; display: flex; align-items: center; justify-content: center; color: white; margin-bottom: 10px; transition: transform 0.3s ease; }
.subject-mini-card:hover .icon-circle { transform: scale(1.1); }
.subject-mini-name { font-size: 0.65rem; font-weight: 800; color: #475569; text-transform: uppercase; text-align: center; padding: 0 5px; }
.active-badge { position: absolute; top: -5px; right: -5px; background: #0d6efd; color: white; width: 22px; height: 22px; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 2px solid white; }
.btn-publish-action { background: #0d6efd; color: white; border: none; transition: 0.3s; }
.btn-publish-action:hover { background: #0b5ed7; transform: translateY(-2px); }
</style>