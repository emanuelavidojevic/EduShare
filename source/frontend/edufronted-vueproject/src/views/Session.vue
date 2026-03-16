<template>
  <div v-if="show" class="session-overlay">
    <div class="session-modal shadow-lg">
      <div class="session-icon-circle">
        <i class="bi bi-shield-lock-fill text-primary"></i>
      </div>
      <h3 class="fw-bold text-dark mb-2">Isticanje sesije</h3>
      <p class="text-muted mb-4">
        Vaša prijava će uskoro isteći. <br>
        Želite li da produžite vreme rada?
      </p>
      <div class="d-flex gap-3 justify-content-center">
        <button @click="$emit('cancel')" class="btn btn-light px-4 py-2 rounded-pill fw-bold border">
          Otkaži
        </button>
        <button @click="$emit('refresh')" class="btn btn-primary px-4 py-2 rounded-pill fw-bold shadow-sm">
          <i class="bi bi-arrow-clockwise me-1"></i> Produži sesiju
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({ show: Boolean });
defineEmits(['refresh', 'cancel']);
</script>

<style scoped>
.session-overlay {
  position: fixed;
  inset: 0;
  background: rgba(13, 110, 253, 0.15);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
}
.session-modal {
  background: white;
  padding: 3rem 2rem;
  border-radius: 30px;
  width: 90%;
  max-width: 450px;
  text-align: center;
  border: 1px solid rgba(255, 255, 255, 0.3);
  animation: modalAppear 0.4s cubic-bezier(0.175, 0.885, 0.32, 1.275);
}
.session-icon-circle {
  width: 80px;
  height: 80px;
  background: #f0f7ff;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 1.5rem;
  font-size: 2.5rem;
}
@keyframes modalAppear {
  from { transform: scale(0.85); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}
</style>