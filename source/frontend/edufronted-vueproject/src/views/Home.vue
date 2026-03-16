<template>
  <div class="home-container animate__animated animate__fadeIn">
    <div class="row mb-4">
      <div class="col-12">
        <div class="hero-card shadow-sm">
          <div class="d-flex align-items-center">
            <div class="avatar-holder me-4">
              <div class="hero-avatar">{{ initials }}</div>
            </div>
            <div class="hero-text">
              <h1 class="welcome-title">Zdravo, {{ user.firstName }}! 👋</h1>
              <div class="d-flex gap-2 mt-2">
                <span class="info-tag"><i class="bi bi-envelope me-2"></i>{{ user.email }}</span>
                <span class="info-tag"><i class="bi bi-building me-2"></i>{{ user.departmentName || 'Student' }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="row g-4 mb-5">
      <div class="col-lg-4">
        <div class="stat-box shadow-sm">
          <h6 class="stat-header"><i class="bi bi-graph-up-arrow me-2 text-warning"></i>Mesečni trend</h6>
          <div class="chart-container"><canvas ref="trendChartRef"></canvas></div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="stat-box shadow-sm">
          <h6 class="stat-header"><i class="bi bi-trophy me-2 text-primary"></i>Najaktivniji predmeti</h6>
          <div class="chart-container"><canvas ref="barChartRef"></canvas></div>
        </div>
      </div>
      <div class="col-lg-4">
        <div class="stat-box shadow-sm">
          <h6 class="stat-header"><i class="bi bi-hdd-stack me-2 text-info"></i>Sadržaj baze</h6>
          <div class="file-stats mt-2">
            <div v-for="(count, type) in fileTypeCounts" :key="type">
              <div v-if="count > 0" class="mb-3">
                <div class="d-flex justify-content-between mb-1">
                  <span class="file-type-label">{{ translateType(Number(type)) }}</span>
                  <span class="file-count">{{ count }} fajla</span>
                </div>
                <div class="progress-mini">
                  <div class="progress-bar-fill" :class="getTypeProgressBarClass(Number(type))" :style="{ width: calculatePercentage(count) + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <h4 class="section-title">Najskorije aktivnosti sistema</h4>
    <div class="table-card shadow-sm">
      <div class="table-responsive">
        <table class="table align-middle mb-0">
          <thead>
            <tr>
              <th class="ps-4">Naziv materijala</th>
              <th>Autor</th>
              <th class="text-center">Tip</th>
              <th>Datum objave</th>
              <th class="text-end pe-4">Opcije</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in materials" :key="item.materialId">
              <td class="ps-4">
                <div class="d-flex align-items-center">
                  <i :class="['bi fs-5 me-3', getFileIcon(item.type)]"></i>
                  <span class="fw-semibold">{{ item.name }}</span>
                </div>
              </td>
              <td>
                <span class="text-dark fw-medium">{{ item.authorName || 'Sistem' }}</span>
              </td>
              <td class="text-center">
                <span class="type-badge">{{ translateType(item.type) }}</span>
              </td>
              <td class="text-muted">{{ formatDate(item.dateCreated) }}</td>
              <td class="text-end pe-4">
                <button @click="downloadFile(item.materialId, item.name)" class="btn-action download" title="Preuzmi">
                  <i class="bi bi-download"></i>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, computed } from 'vue';
import api from '../service/api'; 
import { useAuthStore } from '../stores/auth.js';
import Chart from 'chart.js/auto';

const authStore = useAuthStore();
const materials = ref([]);
const subjects = ref([]);
const trendChartRef = ref(null);
const barChartRef = ref(null);

const user = computed(() => authStore.user || {});

const initials = computed(() => {
  let f = user.value.firstName || '';
  const l = user.value.lastName || '';
  if (f.toLowerCase().startsWith('prof. ')) {
    f = f.split(' ')[1] || '';
  }
  return ((f[0] || '') + (l[0] || '')).toUpperCase();
});

const fileTypeCounts = computed(() => {
  const counts = { 1: 0, 2: 0, 3: 0, 4: 0, 5: 0 };
  materials.value.forEach(m => {
    const t = Number(m.type);
    if (counts[t] !== undefined) counts[t]++; else counts[5]++;
  });
  return counts;
});

const calculatePercentage = (count) => materials.value.length === 0 ? 0 : (count / materials.value.length) * 100;
const getTypeProgressBarClass = (type) => ({ 1: 'bg-danger', 2: 'bg-success', 3: 'bg-warning', 4: 'bg-primary', 5: 'bg-indigo' }[type] || 'bg-secondary');

const fetchData = async () => {
  try {
    const [resMaterials, resSubjects] = await Promise.all([
      api.get('Material'),
      api.get('Subject')
    ]);
    materials.value = resMaterials.data;
    subjects.value = resSubjects.data;
    await nextTick();
    initCharts(resMaterials.data, resSubjects.data);
  } catch (e) { console.error(e); }
};

const initCharts = (allMats, allSubs) => {
  const chartTrend = Chart.getChart(trendChartRef.value);
  const chartBar = Chart.getChart(barChartRef.value);
  if (chartTrend) chartTrend.destroy();
  if (chartBar) chartBar.destroy();

  new Chart(trendChartRef.value, {
    type: 'line',
    data: { 
      labels: ["Jan", "Feb", "Mar", "Apr", "Maj", "Jun", "Jul", "Avg", "Sep", "Okt", "Nov", "Dec"], 
      datasets: [{ label: 'Fajlovi', data: new Array(12).fill(0).map((_, i) => allMats.filter(m => new Date(m.dateCreated).getMonth() === i).length), borderColor: '#0d6efd', backgroundColor: 'rgba(13, 110, 253, 0.1)', fill: true, tension: 0.4 }] 
    },
    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }
  });

  const subjectActivity = allSubs.map(s => ({ name: s.nameSubject, count: allMats.filter(m => m.subjectId === s.subjectId).length }))
    .filter(s => s.count > 0).sort((a, b) => b.count - a.count).slice(0, 5);

  new Chart(barChartRef.value, {
    type: 'bar',
    data: { labels: subjectActivity.map(s => s.name), datasets: [{ data: subjectActivity.map(s => s.count), backgroundColor: '#0d6efd', borderRadius: 6 }] },
    options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }
  });
};

const formatDate = (ds) => ds ? new Date(ds).toLocaleDateString('sr-RS') : "-";
const getFileIcon = (t) => ({ 1: 'bi-file-earmark-pdf text-danger', 2: 'bi-play-circle text-success', 3: 'bi-image text-warning', 4: 'bi-file-earmark-word text-primary', 5: 'bi-terminal text-indigo' }[t] || 'bi-file-earmark');
const translateType = (t) => ({ 1: 'PDF', 2: 'Video', 3: 'Slike', 4: 'Word', 5: 'Kod' }[t] || 'Ostalo');

const downloadFile = async (id, fileName) => {
  try {
    const res = await api.get(`Material/download/${id}`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([res.data]));
    const link = document.createElement('a');
    link.href = url; link.setAttribute('download', fileName);
    document.body.appendChild(link); link.click(); link.remove();
  } catch (e) { alert("Greška pri preuzimanju."); }
};

onMounted(fetchData);
</script>

<style scoped>
.hero-card { background: white; padding: 30px; border-radius: 20px; border: 1px solid #e2e8f0; }
.hero-avatar { width: 80px; height: 80px; border-radius: 15px; background: #0d6efd; color: #ffffff; display: flex; align-items: center; justify-content: center; font-size: 1.8rem; font-weight: 800; border: 2px solid #e2e8f0; box-shadow: 0 4px 12px rgba(13, 110, 253, 0.2); }
.welcome-title { font-size: 1.8rem; font-weight: 800; color: #1e293b; margin: 0; }
.info-tag { font-size: 0.8rem; background: #f1f5f9; padding: 6px 14px; border-radius: 10px; color: #64748b; font-weight: 700; }
.stat-box { background: white; padding: 25px; border-radius: 20px; border: 1px solid #e2e8f0; height: 100%; }
.stat-header { font-weight: 800; color: #94a3b8; text-transform: uppercase; font-size: 0.7rem; letter-spacing: 1px; margin-bottom: 20px; }
.chart-container { height: 180px; }
.progress-mini { height: 6px; background: #f1f5f9; border-radius: 10px; overflow: hidden; }
.progress-bar-fill { height: 100%; border-radius: 10px; }
.section-title { font-weight: 800; color: #1e293b; margin-top: 40px; margin-bottom: 20px; font-size: 1.1rem; }
.table-card { background: white; border-radius: 20px; border: 1px solid #e2e8f0; overflow: hidden; margin-bottom: 50px; }
.table thead th { background: #f8fafc; color: #64748b; font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.5px; border-bottom: 1px solid #e2e8f0; padding: 15px 10px; }
.type-badge { background: #f1f5f9; padding: 5px 12px; border-radius: 20px; font-size: 0.7rem; font-weight: 800; color: #475569; }
.btn-action { width: 32px; height: 32px; border-radius: 8px; border: none; margin-left: 5px; transition: 0.2s; }
.btn-action.download { background: #e0f2fe; color: #0284c7; }
.btn-action:hover { transform: translateY(-2px); filter: brightness(0.95); }
</style>