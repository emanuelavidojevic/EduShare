<template>
  <div class="container-fluid py-4">
    <div class="row g-4">
      
      <div class="col-lg-5 col-xl-4">
        <div class="card border-0 shadow-sm h-100 rounded-4">
          <div class="card-body p-4 text-center">
            <div class="avatar-container mx-auto mb-4">
              <div class="avatar-box bg-primary shadow-sm overflow-hidden">
                <img v-if="profileImage" :src="profileImage" class="w-100 h-100 object-fit-cover" />
                <i v-else class="bi bi-person-fill text-white fs-1"></i>
              </div>
              <button class="edit-photo-btn shadow-sm">
                <i class="bi bi-pencil-fill"></i>
              </button>
            </div>
            
            <h4 class="fw-bold mb-1">Milica Jovanović</h4>
            <p class="text-muted small mb-4">Student</p>

            <div class="info-list text-start mb-4">
              <div class="info-item border-bottom py-3">
                <span class="text-muted tiny-label">Email</span>
                <span class="fw-semibold d-block text-dark">milica@edu.rs</span>
              </div>
              <div class="info-item border-bottom py-3">
                <span class="text-muted tiny-label">Smer</span>
                <span class="fw-semibold d-block text-dark">Informacione Tehnologije</span>
              </div>
              <div class="info-item py-3">
                <span class="text-muted tiny-label">Broj indeksa</span>
                <span class="fw-semibold d-block text-dark">2021/0042</span>
              </div>
            </div>

            <div class="mt-4 text-start">
              <h6 class="fw-bold mb-3 small text-uppercase">Moje slike iz baze (BLOB):</h6>
              <div class="d-flex flex-wrap gap-2">
                <div v-for="(img, index) in userImages" :key="index" class="blob-img-wrapper">
                  <img :src="img" class="rounded shadow-sm border" style="width: 55px; height: 55px; object-fit: cover;" />
                </div>
                <div v-if="userImages.length === 0" class="text-muted small italic">Nema slika u MaterialData.</div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-7 col-xl-8">
        <div class="row g-4">
          
          <div class="col-12">
            <div class="card border-0 shadow-sm rounded-4 p-4 bg-white">
              <div class="d-flex justify-content-between align-items-center mb-4">
                <h5 class="fw-bold mb-0">Popularnost predmeta</h5>
                <span class="badge bg-primary-subtle text-primary rounded-pill px-3">Real-time</span>
              </div>
              <div class="activity-chart">
                <div v-for="(item, index) in subjectStats" :key="index" class="mb-3">
                  <div class="d-flex justify-content-between mb-1 small">
                    <span class="fw-medium">{{ item.name }}</span>
                    <span class="text-muted">{{ item.comments }} komentara</span>
                  </div>
                  <div class="progress rounded-pill" style="height: 10px;">
                    <div class="progress-bar bg-primary" :style="{ width: item.popularity + '%' }"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6">
            <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100">
              <h5 class="fw-bold mb-4">Zastupljenost predmeta</h5>
              <div class="donut-container mx-auto mb-4">
                <div class="donut-chart" :style="donutGradient">
                  <div class="donut-center">
                    <span class="fs-4 fw-bold">{{ totalMaterials }}</span>
                    <span class="tiny-text d-block">Fajlova</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="col-md-6">
            <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100">
              <h5 class="fw-bold mb-4">Tip materijala (Iz baze)</h5>
              <div v-if="loading" class="text-center py-5">
                <div class="spinner-border text-primary spinner-border-sm"></div>
              </div>
              <div v-else class="mt-2">
                <div v-for="stat in materialStats" :key="stat.typeName" class="material-stat mb-4">
                  <div class="d-flex align-items-center gap-3">
                    <div class="icon-circle" :class="getTheme(stat.typeName).bg">
                      <i class="bi" :class="getTheme(stat.typeName).icon"></i>
                    </div>
                    <div class="w-100">
                      <div class="d-flex justify-content-between mb-1">
                        <span class="fw-bold small">{{ stat.typeName }}</span>
                        <span class="fw-bold small" :class="getTheme(stat.typeName).text">{{ stat.percentage }}%</span>
                      </div>
                      <div class="progress rounded-pill" style="height: 6px;">
                        <div class="progress-bar" :class="getTheme(stat.typeName).bar" :style="{ width: stat.percentage + '%' }"></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';

const materialStats = ref([]);
const userImages = ref([]);
const loading = ref(true);
const totalMaterials = ref(0);

const profileImage = computed(() => userImages.value.length > 0 ? userImages.value[0] : null);

const subjectStats = [
  { name: 'C# Programiranje', popularity: 95, comments: 48 },
  { name: 'Baze Podataka', popularity: 80, comments: 32 }
];

const fetchData = async () => {
  try {
    loading.value = true;
    const userId = 2; // UserId iz tvoje baze
    
    const [statsRes, imagesRes] = await Promise.all([
      axios.get(`api/Material/stats/${userId}`),
      axios.get(`api/Material/user-images/${userId}`)
    ]);

    materialStats.value = statsRes.data;
    userImages.value = imagesRes.data;
    totalMaterials.value = materialStats.value.reduce((acc, curr) => acc + curr.count, 0);
  } catch (error) {
    console.error("Greška:", error);
  } finally {
    loading.value = false;
  }
};

const donutGradient = computed(() => {
  if (materialStats.value.length === 0) return 'background: #eee';
  return 'background: conic-gradient(#0d6efd 0% 50%, #198754 50% 80%, #ffc107 80% 100%)';
});

const getTheme = (typeName) => {
  const themes = {
    'Pdf': { icon: 'bi-file-earmark-pdf text-danger', bg: 'bg-light-danger', bar: 'bg-danger', text: 'text-danger' },
    'Video': { icon: 'bi-camera-video text-success', bg: 'bg-light-success', bar: 'bg-success', text: 'text-success' },
    'Image': { icon: 'bi-image text-info', bg: 'bg-light-info', bar: 'bg-info', text: 'text-info' },
    'Document': { icon: 'bi-file-earmark-word text-primary', bg: 'bg-light-primary', bar: 'bg-primary', text: 'text-primary' }
  };
  return themes[typeName] || { icon: 'bi-file-earmark', bg: 'bg-light', bar: 'bg-secondary', text: 'text-dark' };
};

onMounted(fetchData);
</script>

<style scoped>
.avatar-container { position: relative; width: 110px; height: 110px; }
.avatar-box { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 4px solid #fff; }
.edit-photo-btn { position: absolute; bottom: 5px; right: 5px; width: 32px; height: 32px; background: white; border: none; border-radius: 50%; color: #0d6efd; display: flex; align-items: center; justify-content: center; }
.tiny-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; color: #aaa; }
.info-list { background-color: #f9f9f9; padding: 10px 20px; border-radius: 16px; }
.donut-container { width: 160px; height: 160px; }
.donut-chart { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.donut-center { width: 70%; height: 70%; background: white; border-radius: 50%; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.icon-circle { width: 42px; height: 42px; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.bg-light-danger { background: #fee2e2; }
.bg-light-success { background: #dcfce7; }
.bg-light-primary { background: #dbeafe; }
.bg-light-info { background: #e0f7fa; }
.tiny-text { font-size: 0.7rem; color: #999; text-transform: uppercase; }
</style>