<template>
  <div class="container-fluid py-4">
    <div class="row justify-content-center">
      <div class="col-md-8 col-lg-6">
        <div class="card border-0 shadow-sm profile-card">
          <div class="card-body p-5">
            <div class="text-center mb-4">
              <div class="profile-avatar-large mx-auto mb-3">
                {{ initials }}
              </div>
              <h2 class="fw-bold mb-1">{{ user.firstName }} {{ user.lastName }}</h2>
              <span class="badge bg-primary-soft text-primary rounded-pill px-3 py-2">
                <i class="bi bi-person-badge me-1"></i> {{ user.role }}
              </span>
            </div>

            <hr class="my-4 opacity-25">

            <div class="profile-info-grid">
              <div class="info-item mb-4">
                <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Email Adresa</label>
                <div class="d-flex align-items-center">
                  <i class="bi bi-envelope text-primary me-3 fs-5"></i>
                  <span class="fs-5 fw-medium">{{ user.email || 'Nije podešeno' }}</span>
                </div>
              </div>

              <div class="info-item mb-4">
                <label class="text-muted small text-uppercase fw-bold mb-1 d-block">Smer / Departman</label>
                <div class="d-flex align-items-center">
                  <i class="bi bi-mortarboard text-primary me-3 fs-5"></i>
                  <span class="fs-5 fw-medium">{{ user.departmentName || 'Nema smera' }}</span>
                </div>
              </div>
            </div>

            <div class="mt-5 d-grid">
              <button @click="editProfile" class="btn btn-outline-primary btn-lg rounded-pill fw-bold">
                <i class="bi bi-pencil-square me-2"></i> Izmeni profil
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';

const user = ref({
  firstName: '',
  lastName: '',
  email: '',
  departmentName: '',
  role: ''
});

const loadProfile = () => {
  const data = localStorage.getItem('user_display');
  if (data) {
    user.value = JSON.parse(data);
  }
};

const initials = computed(() => {
  const f = user.value.firstName ? user.value.firstName.charAt(0).toUpperCase() : '';
  const l = user.value.lastName ? user.value.lastName.charAt(0).toUpperCase() : '';
  return (f + l) || '??';
});

const editProfile = () => {
  alert('Funkcija za izmenu će biti dostupna uskoro!');
};

onMounted(() => {
  loadProfile();
  // Slušamo promene ako DashboardLayout osveži podatke u pozadini
  window.addEventListener('storage-updated', loadProfile);
});
</script>

<style scoped>
.profile-card {
  border-radius: 24px;
  background: white;
}

.profile-avatar-large {
  width: 100px;
  height: 100px;
  background-color: #f0f7ff;
  color: #0d6efd;
  border: 4px solid #0d6efd;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 2.5rem;
  font-weight: 800;
}

.bg-primary-soft {
  background-color: #e7f1ff;
}

.profile-info-grid .info-item {
  padding: 10px;
  transition: 0.2s;
}

.btn-outline-primary:hover {
  background-color: #0d6efd;
  color: white;
}
</style>