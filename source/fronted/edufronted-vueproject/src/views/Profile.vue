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
            <p class="text-muted small mb-3">{{ currentUser.role }}</p>
            <div class="info-list text-start">
              <div class="info-item border-bottom py-2">
                <span class="text-muted tiny-label d-block">Email</span>
                <span class="fw-semibold text-dark">{{ currentUser.email || 'Nema podataka' }}</span>
              </div>
              <div class="info-item border-bottom py-2">
                <span class="text-muted tiny-label d-block">Smer</span>
                <span class="fw-semibold text-dark">{{ currentUser.departmentName }}</span>
              </div>
              <div class="info-item pt-3 text-center">
                <span class="badge bg-success-subtle text-success rounded-pill px-3 py-2">Aktivan Nalog</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-7 col-xl-8">
        <div class="card border-0 shadow-sm h-100 rounded-4 p-4 bg-white">
          <h5 class="fw-bold mb-4 text-primary"><i class="bi bi-bar-chart-fill me-2"></i>Popularnost predmeta</h5>
          <div v-if="loading" class="text-center py-4">
            <div class="spinner-border text-primary" role="status"></div>
          </div>
          <div v-else class="activity-chart">
            <div v-if="subjectStats.length === 0" class="text-center text-muted py-3">Nema podataka za prikaz.</div>
            <div v-for="(item, index) in subjectStats" :key="index" class="mb-3">
              <div class="d-flex justify-content-between mb-1 small">
                <span class="fw-bold text-dark">{{ item.label }}</span>
                <span class="badge bg-light text-dark border">{{ item.count }} materijala</span>
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
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100 text-center">
          <h5 class="fw-bold mb-4">Zastupljenost</h5>
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-border text-primary" role="status"></div>
          </div>
          <div v-else class="d-flex flex-column align-items-center justify-content-center h-100">
            <div class="donut-container mb-3">
              <div class="donut-chart shadow-sm" :style="donutGradient">
                <div class="donut-center">
                  <span class="fs-2 fw-bold text-dark">{{ totalMaterials }}</span>
                  <span class="tiny-text d-block text-muted">FAJLOVA</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-6">
        <div class="card border-0 shadow-sm rounded-4 p-4 bg-white h-100">
          <h5 class="fw-bold mb-4">Tipovi fajlova</h5>
          <div v-if="loading" class="text-center py-5">
            <div class="spinner-border text-primary" role="status"></div>
          </div>
          <div v-else>
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
    </div>

    <div class="row">
      <div class="col-12">
        <div class="card border-0 shadow-sm rounded-4 overflow-hidden bg-white">
          <div class="card-header bg-white border-0 py-3 d-flex justify-content-between align-items-center flex-wrap gap-2">
            <h5 class="fw-bold mb-0 text-primary"><i class="bi bi-folder2-open me-2"></i>Moji Materijali</h5>
            <div class="d-flex gap-2">
              <input v-model="profileParams.searchTerm" @input="debounceProfileSearch" type="text" class="form-control form-control-sm" placeholder="Pretraži moje fajlove..." style="max-width: 200px;">
            </div>
          </div>
          
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="bg-light small text-muted text-uppercase">
                <tr>
                  <th class="ps-4">Materijal</th>
                  <th>Predmet</th>
                  <th>Datum</th>
                  <th class="text-end pe-4">Akcije</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="file in myMaterials" :key="file.materialId">
                  <td class="ps-4">
                    <router-link :to="'/dashboard/subject/' + file.subjectId" class="text-decoration-none fw-bold text-dark">
                      {{ file.name }}
                    </router-link>
                  </td>
                  <td>
                    <span class="badge bg-primary-subtle text-primary fw-medium">
                      {{ file.subjectName }}
                    </span>
                  </td>
                  <td class="small text-muted">{{ formatDate(file.dateCreated) }}</td>
                  <td class="text-end pe-4">
                    <button @click="deleteFile(file.materialId)" class="btn btn-sm btn-outline-danger border-0">
                      <i class="bi bi-trash"></i>
                    </button>
                  </td>
                </tr>
                <tr v-if="myMaterials.length === 0">
                  <td colspan="4" class="text-center py-4 text-muted small">Niste još uvek postavili nijedan materijal.</td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="d-flex justify-content-center p-3 border-top bg-light-subtle gap-3">
            <button @click="changeProfilePage(-1)" :disabled="profileParams.page === 1" class="btn btn-sm btn-white border shadow-sm px-3">
              <i class="bi bi-chevron-left"></i> Prethodna
            </button>
            <button @click="changeProfilePage(1)" :disabled="myMaterials.length < profileParams.pageSize" class="btn btn-sm btn-white border shadow-sm px-3">
              Sledeća <i class="bi bi-chevron-right"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';

interface StatItem { label: string; count: number; percentage: number; }

const materialStats = ref<StatItem[]>([]);
const subjectStats = ref<StatItem[]>([]);
const totalMaterials = ref(0);
const loading = ref(true);
const colors = ['#0d6efd', '#198754', '#ffc107', '#dc3545', '#6610f2', '#fd7e14'];

const myMaterials = ref([]);
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

const fetchData = async () => {
  try {
    loading.value = true;
    // 1. Prvo dobavljamo podatke o ulogovanom korisniku
    const userRes = await axios.get('http://localhost:5145/api/Auth/me', { withCredentials: true });
    
    if (userRes.data) {
      currentUser.value = {
        firstName: userRes.data.firstName,
        lastName: userRes.data.lastName,
        email: userRes.data.email,
        role: userRes.data.role,
        departmentName: userRes.data.departmentName,
        userId: userRes.data.userId
      };

      profileParams.value.userId = userRes.data.userId;

      // Čuvanje osnovnih informacija u localStorage za UI sinhronizaciju
      localStorage.setItem('user_display', JSON.stringify({ 
        firstName: userRes.data.firstName, 
        lastName: userRes.data.lastName,
        userId: userRes.data.userId
      }));
      window.dispatchEvent(new Event('storage-updated'));

      // 2. Povlačimo statistiku FILTRIRANU po departmanu korisnika
      // Šaljemo departmentName kao query parametar da bi backend vratio predmete tog smera
      const statsRes = await axios.get(`http://localhost:5145/api/Material/dashboard-stats/${currentUser.value.userId}`, { 
        params: { departmentName: currentUser.value.departmentName },
        withCredentials: true 
      });

      if (statsRes.data) {
        // Tipovi fajlova (Pdf, Image...) ostaju generalni za korisnika
        materialStats.value = statsRes.data.types || [];
        
        // Predmeti su sada oni koji pripadaju departmanu, sa brojem materijala na njima
        subjectStats.value = statsRes.data.subjects || [];
        
        totalMaterials.value = statsRes.data.totalCount || 0;
      }

      // 3. Povuci listu materijala koje je okačio baš taj korisnik
      fetchMyMaterials();
    }
  } catch (error) {
    console.error("Greška pri inicijalizaciji dashboard-a:", error);
  } finally {
    loading.value = false;
  }
};

const fetchMyMaterials = async () => {
  try {
    const res = await axios.get('http://localhost:5145/api/Material', { 
      params: profileParams.value,
      withCredentials: true 
    });
    myMaterials.value = res.data;
  } catch (e) {
    console.error("Greška pri dobavljanju tvojih materijala:", e);
  }
};

const deleteFile = async (id: number) => {
  if (!confirm("Da li ste sigurni da želite da obrišete ovaj materijal?")) return;
  try {
    await axios.delete(`http://localhost:5145/api/Material/${id}`, { withCredentials: true });
    // Nakon brisanja, osvežavamo i tabelu i statistiku jer se broj fajlova promenio
    fetchData(); 
  } catch (e) {
    alert("Greška pri brisanju materijala.");
  }
};

// Paginacija i Search
const changeProfilePage = (step: number) => {
  profileParams.value.page += step;
  fetchMyMaterials();
};

let timeout: any = null;
const debounceProfileSearch = () => {
  clearTimeout(timeout);
  timeout = setTimeout(() => {
    profileParams.value.page = 1;
    fetchMyMaterials();
  }, 500);
};

// Dinamički gradijent za Donut grafikon na osnovu predmetne statistike
const donutGradient = computed(() => {
  if (!subjectStats.value.length || totalMaterials.value === 0) return 'background: #f0f0f0';
  let pos = 0;
  const segs = subjectStats.value
    .filter(s => s.percentage > 0) // Samo predmeti koji imaju materijale ulaze u krug
    .map((s, i) => {
      const start = pos;
      pos += s.percentage;
      return `${colors[i % colors.length]} ${start}% ${pos}%`;
    });
  
  return segs.length > 0 
    ? `background: conic-gradient(${segs.join(', ')})` 
    : 'background: #f0f0f0';
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

const formatDate = (dateStr: string) => {
  if (!dateStr) return 'N/A';
  return new Date(dateStr).toLocaleDateString('sr-RS');
};

onMounted(fetchData);
</script>

<style scoped>
.avatar-container { position: relative; width: 110px; height: 110px; }
.avatar-box { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; border: 4px solid #fff; }
.tiny-label { font-size: 0.65rem; font-weight: 700; text-transform: uppercase; color: #999; }
.info-list { background-color: #fcfcfc; padding: 10px 20px; border-radius: 20px; border: 1px solid #f0f0f0; }
.donut-container { width: 160px; height: 160px; }
.donut-chart { width: 100%; height: 100%; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
.donut-center { width: 75%; height: 75%; background: white; border-radius: 50%; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.icon-circle { width: 42px; height: 42px; border-radius: 10px; display: flex; align-items: center; justify-content: center; }
.bg-light-danger { background: #fff1f2; }
.bg-light-warning { background: #fffbeb; }
.bg-light-success { background: #f0fdf4; }
.bg-light-primary { background: #eff6ff; }
.btn-white { background: white; color: #555; }
.btn-white:hover:not(:disabled) { background: #f8f9fa; }
</style>