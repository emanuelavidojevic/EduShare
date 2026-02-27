<!--<template>
  <div class="container-fluid py-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h3 class="fw-bold text-dark m-0">
        <i class="bi bi-bell-fill text-primary me-2"></i>Aktivnosti na platformi
      </h3>
      <button @click="fetchNotifications" class="btn btn-light btn-sm rounded-pill px-3 border">
        <i class="bi bi-arrow-clockwise me-1"></i> Osveži
      </button>
    </div>

    <div class="card border-0 shadow-sm rounded-4 overflow-hidden">
      <div class="list-group list-group-flush">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-border text-primary" role="status"></div>
          <p class="mt-2 text-muted">Učitavanje obaveštenja...</p>
        </div>

        <div v-for="note in notifications" :key="note.id" class="list-group-item p-4 border-start-4" :class="getBorderClass(note.type)">
          <div class="d-flex gap-3">
            <div :class="['icon-box rounded-circle d-flex align-items-center justify-content-center shadow-sm', getIconBg(note.type)]" style="width: 45px; height: 45px; flex-shrink: 0;">
              <i :class="['bi text-white fs-5', getIcon(note.type)]"></i>
            </div>
            <div class="flex-grow-1">
              <div class="d-flex justify-content-between align-items-start">
                <h6 class="fw-bold mb-1 text-dark">{{ note.title }}</h6>
                <span class="badge bg-light text-muted fw-normal">{{ formatTime(note.timestamp) }}</span>
              </div>
              <p class="mb-0 text-secondary small">{{ note.message }}</p>
              <div class="mt-2">
                <span class="small text-primary fw-semibold cursor-pointer" @click="navigateToChange(note)">
                  Pogledaj detalje <i class="bi bi-chevron-right small"></i>
                </span>
              </div>
            </div>
          </div>
        </div>

        <div v-if="!loading && notifications.length === 0" class="text-center py-5">
          <i class="bi bi- mailbox2 fs-1 text-muted d-block mb-2"></i>
          <p class="text-muted">Trenutno nema novih aktivnosti.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const notifications = ref([]);
const loading = ref(true);

const fetchNotifications = async () => {
  try {
    loading.value = true;
    // Napomena: Ovaj endpoint treba da vrati listu logova (npr. AuditLogs ili Notifications tabelu)
    const res = await axios.get('http://localhost:5145/api/Notification', { withCredentials: true });
    notifications.value = res.data;
  } catch (e) {
    console.error("Greška pri učitavanju obaveštenja", e);
  } finally {
    loading.value = false;
  }
};

const getIcon = (type) => {
  const icons = {
    'Upload': 'bi-cloud-arrow-up',
    'Delete': 'bi-trash3',
    'SubjectCreated': 'bi-journal-plus',
    'Update': 'bi-pencil-square'
  };
  return icons[type] || 'bi-info-circle';
};

const getIconBg = (type) => {
  const bgs = {
    'Upload': 'bg-success',
    'Delete': 'bg-danger',
    'SubjectCreated': 'bg-primary',
    'Update': 'bg-warning'
  };
  return bgs[type] || 'bg-secondary';
};

const getBorderClass = (type) => {
  const classes = {
    'Upload': 'border-start-success',
    'Delete': 'border-start-danger',
    'SubjectCreated': 'border-start-primary',
    'Update': 'border-start-warning'
  };
  return classes[type] || 'border-start-secondary';
};

const formatTime = (date) => {
  return new Date(date).toLocaleString('sr-RS', { 
    day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' 
  });
};

const navigateToChange = (note) => {
  if (note.subjectId) {
    router.push(`/dashboard/subject/${note.subjectId}`);
  }
};

onMounted(fetchNotifications);
</script>

<style scoped>
.border-start-4 { border-left: 4px solid transparent !important; }
.border-start-success { border-left-color: #198754 !important; }
.border-start-danger { border-left-color: #dc3545 !important; }
.border-start-primary { border-left-color: #0d6efd !important; }
.border-start-warning { border-left-color: #ffc107 !important; }
.cursor-pointer { cursor: pointer; }
.list-group-item:hover { background-color: #f8f9fa; }
</style>-->