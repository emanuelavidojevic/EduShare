<template>
  <div class="subjects-page">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="fw-bold text-dark">Moji Predmeti</h2>
      <button class="btn btn-primary shadow-sm rounded-pill px-4" @click="showAddModal = true">
        <i class="bi bi-plus-lg me-2"></i>Dodaj novi predmet
      </button>
    </div>

    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="mt-2 text-muted">Učitavanje predmeta...</p>
    </div>

    <div v-else class="row g-4">
      <div v-for="subject in subjects" :key="subject.subjectId" class="col-md-6 col-lg-4">
        <div class="card h-100 subject-card shadow-sm border-0" @click="goToSubject(subject.subjectId)">
          <div class="card-body text-center p-4">
            <div :class="['icon-box mb-3', getDesign(subject.nameSubject).bg]">
              <i :class="['bi', getDesign(subject.nameSubject).icon, getDesign(subject.nameSubject).text, 'fs-1']"></i>
            </div>
            <h5 class="fw-bold">{{ subject.nameSubject }}</h5>
            <p class="text-muted small">{{ subject.descriptionSubject }}</p>
            
            <div class="mt-3">
              <span class="badge rounded-pill bg-light text-primary border px-3">
                <i class="bi bi-files me-1"></i> {{ subject.materialCount }} materijala
              </span>
            </div>
          </div>
        </div>
      </div>

      <div v-if="subjects.length === 0" class="text-center py-5">
        <i class="bi bi-journal-x display-1 text-muted"></i>
        <p class="mt-3 text-muted">Nema dostupnih predmeta za vaš smer.</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '../service/api';

const router = useRouter();
const subjects = ref([]);
const isLoading = ref(true);
const showAddModal = ref(false); 

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

const goToSubject = (id) => {
  router.push(`/dashboard/subject/${id}`);
};

const getDesign = (name) => {
  const n = name ? name.toLowerCase() : '';
  if (n.includes('c#')) return { icon: 'bi-hash', bg: 'bg-purple-light', text: 'text-purple' };
  if (n.includes('baze')) return { icon: 'bi-database', bg: 'bg-orange-light', text: 'text-orange' };
  if (n.includes('internet')) return { icon: 'bi-globe', bg: 'bg-green-light', text: 'text-success' };
  if (n.includes('python')) return { icon: 'bi-code-slash', bg: 'bg-blue-light', text: 'text-primary' };
  return { icon: 'bi-book', bg: 'bg-light', text: 'text-dark' };
};

onMounted(fetchSubjects);
</script>

<style scoped>
.subject-card { transition: all 0.3s ease; cursor: pointer; border-radius: 20px; }
.subject-card:hover { transform: translateY(-10px); box-shadow: 0 15px 30px rgba(0,0,0,0.1) !important; }
.icon-box { width: 85px; height: 85px; margin: 0 auto; display: flex; align-items: center; justify-content: center; border-radius: 22px; }
.bg-purple-light { background-color: #f3e5f5; } .text-purple { color: #9c27b0; }
.bg-blue-light { background-color: #e3f2fd; } .text-blue { color: #2196f3; }
.bg-orange-light { background-color: #fff3e0; } .text-orange { color: #ff9800; }
.bg-green-light { background-color: #e8f5e9; }
</style>