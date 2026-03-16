<template>
  <div class="notification-wrapper animate__animated animate__fadeIn">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="fw-bold text-dark m-0">
        <i class="bi bi-bell-fill text-primary me-2"></i>Aktivnosti sistema
      </h4>
      <button @click="fetchNotifications" class="btn btn-white shadow-sm rounded-pill px-4 border" :disabled="loading">
        <i class="bi bi-arrow-clockwise me-1"></i> Osveži
      </button>
    </div>

    <div class="card border-0 shadow-sm rounded-4 overflow-hidden bg-white">
      <div class="list-group list-group-flush">
        <div v-if="loading" class="text-center py-5">
          <div class="spinner-grow text-primary"></div>
        </div>

        <div v-else-if="notifications.length > 0">
          <div v-for="note in notifications" :key="note.id" 
               class="list-group-item p-4 border-start-4 transition-item" 
               :class="getBorderClass(note.typeChange)">
            <div class="d-flex gap-3">
              <div :class="['icon-box rounded-4 d-flex align-items-center justify-content-center', getIconBg(note.typeChange)]" 
                   style="width: 50px; height: 50px; flex-shrink: 0;">
                <i :class="['bi text-white fs-4', getIcon(note.typeChange)]"></i>
              </div>

              <div class="flex-grow-1">
                <div class="d-flex justify-content-between align-items-start">
                  <div>
                    <h6 class="fw-bold mb-1 text-dark">
                      {{ note.title }} 
                      <span v-if="!note.isRead" class="badge bg-primary ms-2" style="font-size: 0.6rem;">NOVO</span>
                    </h6>
                    <p class="mb-0 text-secondary small">{{ note.message }}</p>
                  </div>
                  <span class="badge bg-light text-muted border fw-normal">{{ formatTime(note.timestamp) }}</span>
                </div>

                <div class="mt-3 d-flex align-items-center justify-content-between">
                  <span class="action-link" @click="handleNavigation(note)">
                    Pogledaj detalje <i class="bi bi-arrow-right ms-1"></i>
                  </span>
                  <span class="type-tag" :class="getLightBgClass(note.typeChange)">
                    <i class="bi bi-person-circle me-1"></i> {{ note.authorName || 'Sistem' }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div v-else class="text-center py-5">
          <i class="bi bi-chat-square-dots fs-1 text-muted"></i>
          <p class="mt-3 text-muted">Nema novih obaveštenja.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import api from '../service/api'; 

const router = useRouter();
const notifications = ref([]);
const loading = ref(true);

const fetchNotifications = async () => {
  try {
    loading.value = true;
    const res = await api.get('Notifications');
    notifications.value = res.data;
    
    // Automatsko markiranje kao pročitano
    await api.post('Notifications/mark-as-read');
  } catch (e) { 
    console.error("Greška pri učitavanju obaveštenja:", e); 
  } finally { 
    loading.value = false; 
  }
};

const handleNavigation = (note) => {
  if (note.materialId && note.subjectId) {
    router.push({ 
      path: `/dashboard/subject/${note.subjectId}`, 
      query: { openMaterial: note.materialId } 
    });
  } else if (note.subjectId) {
    router.push(`/dashboard/subject/${note.subjectId}`);
  }
};

// UI Helperi
const getIcon = (t) => ({ 
  'Upload': 'bi-cloud-arrow-up-fill', 
  'Comment': 'bi-chat-left-text-fill', 
  'Delete': 'bi-trash3-fill', 
  'SubjectCreated': 'bi-journal-plus', 
  'Update': 'bi-pencil-square' 
}[t] || 'bi-info-circle-fill');

const getIconBg = (t) => ({ 
  'Upload': 'bg-success', 
  'Comment': 'bg-info', 
  'Delete': 'bg-danger', 
  'SubjectCreated': 'bg-primary', 
  'Update': 'bg-warning' 
}[t] || 'bg-secondary');

const getLightBgClass = (t) => ({
  'Upload': 'bg-success-light',
  'Comment': 'bg-info-light',
  'Delete': 'bg-danger-light',
  'SubjectCreated': 'bg-primary-light',
  'Update': 'bg-warning-light'
}[t] || 'bg-light');

const getBorderClass = (t) => ({ 
  'Upload': 'border-start-success', 
  'Comment': 'border-start-info', 
  'Delete': 'border-start-danger', 
  'SubjectCreated': 'border-start-primary', 
  'Update': 'border-start-warning' 
}[t] || 'border-start-secondary');

const formatTime = (d) => d ? new Date(d).toLocaleString('sr-RS', { 
  day: '2-digit', 
  month: '2-digit', 
  hour: '2-digit', 
  minute: '2-digit' 
}) : "";

onMounted(fetchNotifications);
</script>

<style scoped>
.notification-wrapper { max-width: 900px; margin: 0 auto; }
.border-start-4 { border-left: 5px solid transparent !important; }
.border-start-success { border-left-color: #198754 !important; }
.border-start-info { border-left-color: #0dcaf0 !important; }
.border-start-warning { border-left-color: #ffc107 !important; }
.border-start-danger { border-left-color: #dc3545 !important; }
.border-start-primary { border-left-color: #0d6efd !important; }

.action-link { 
  font-size: 0.8rem; 
  font-weight: 800; 
  color: #0d6efd; 
  cursor: pointer; 
  text-transform: uppercase; 
  transition: 0.2s; 
}
.action-link:hover { color: #0a58ca; letter-spacing: 0.5px; }

.type-tag { font-size: 0.7rem; font-weight: 800; padding: 6px 14px; border-radius: 20px; }

.bg-success-light { background-color: #d1e7dd; color: #0f5132; }
.bg-info-light { background-color: #cff4fc; color: #055160; }
.bg-warning-light { background-color: #fff3cd; color: #664d03; }
.bg-danger-light { background-color: #f8d7da; color: #842029; }
.bg-primary-light { background-color: #cfe2ff; color: #084298; }

.btn-white { background: white; transition: all 0.2s; }
.btn-white:hover { background: #f8f9fa; transform: translateY(-1px); }

.transition-item { transition: all 0.2s ease; }
.transition-item:hover { background-color: #fcfcfc; transform: scale(1.005); }
</style>