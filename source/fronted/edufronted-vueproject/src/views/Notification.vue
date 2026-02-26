<script setup>
import { ref } from 'vue';

// Primer podataka (ovo bi inače dolazilo sa API-ja)
const notifications = ref([
  {
    id: 1,
    title: "Novi materijal postavljen",
    message: "Profesor je dodao novu lekciju u predmetu 'Programiranje 1'.",
    time: "Pre 5 minuta",
    type: "info",
    isRead: false
  },
  {
    id: 2,
    title: "Promena termina vežbi",
    message: "Vežbe iz 'Matematike' su pomerene za četvrtak u 12:00h.",
    time: "Pre 2 sata",
    type: "warning",
    isRead: false
  },
  {
    id: 3,
    title: "Uspešna prijava",
    message: "Uspešno ste se prijavili na kurs 'Web Dizajn'.",
    time: "Juče",
    type: "success",
    isRead: true
  }
]);

const markAsRead = (id) => {
  const notif = notifications.value.find(n => n.id === id);
  if (notif) notif.isRead = true;
};

const markAllAsRead = () => {
  notifications.value.forEach(n => n.isRead = true);
};
</script>

<template>
  <div class="notifications-container">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="fw-bold m-0">Obaveštenja</h2>
      <button 
        @click="markAllAsRead" 
        class="btn btn-outline-primary btn-sm"
        v-if="notifications.some(n => !n.isRead)"
      >
        Označi sve kao pročitano
      </button>
    </div>

    <div class="card shadow-sm border-0">
      <div class="list-group list-group-flush">
        <div 
          v-for="notif in notifications" 
          :key="notif.id"
          class="list-group-item list-group-item-action p-3 notification-item"
          :class="{ 'unread': !notif.isRead }"
          @click="markAsRead(notif.id)"
        >
          <div class="d-flex align-items-start">
            <div :class="['icon-circle me-3', notif.type]">
              <i v-if="notif.type === 'info'" class="bi bi-info-circle"></i>
              <i v-if="notif.type === 'warning'" class="bi bi-exclamation-triangle"></i>
              <i v-if="notif.type === 'success'" class="bi bi-check-circle"></i>
            </div>

            <div class="flex-grow-1">
              <div class="d-flex justify-content-between align-items-center">
                <h6 class="mb-1 fw-bold">{{ notif.title }}</h6>
                <small class="text-muted">{{ notif.time }}</small>
              </div>
              <p class="mb-0 text-secondary small">{{ notif.message }}</p>
            </div>
            
            <div v-if="!notif.isRead" class="unread-dot ms-2"></div>
          </div>
        </div>

        <div v-if="notifications.length === 0" class="p-5 text-center">
          <i class="bi bi-bell-slash text-muted display-4"></i>
          <p class="mt-3 text-muted">Trenutno nemate novih obaveštenja.</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.notifications-container {
  max-width: 900px;
  margin: 0 auto;
}

.notification-item {
  cursor: pointer;
  transition: 0.2s;
  border-left: 4px solid transparent;
}

.notification-item.unread {
  background-color: #f0f7ff;
  border-left-color: #0d6efd;
}

.notification-item:hover {
  background-color: #f8f9fa;
}

.icon-circle {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.icon-circle i {
  font-size: 1.2rem;
}

/* Boje ikona */
.info { background-color: #e7f1ff; color: #0d6efd; }
.warning { background-color: #fff3cd; color: #856404; }
.success { background-color: #d1e7dd; color: #0f5132; }

.unread-dot {
  width: 10px;
  height: 10px;
  background-color: #0d6efd;
  border-radius: 50%;
  align-self: center;
}

.card {
  border-radius: 12px;
  overflow: hidden;
}
</style>