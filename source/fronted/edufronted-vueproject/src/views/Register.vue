<template>
  <div class="register-page">
    <div class="cloud cloud-1"></div>
    <div class="cloud cloud-2"></div>
    <div class="cloud cloud-3"></div>

    <div v-if="isLoading" class="loading-overlay">
      <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;"></div>
      <h5 class="mt-3 fw-bold text-primary animate-pulse">Kreiramo tvoj nalog...</h5>
    </div>

    <div class="container py-5" :class="{ 'blur-content': isLoading }">
      <div class="row justify-content-center">
        <div class="col-12 col-md-10 col-lg-8">
          <div class="card register-card shadow-lg border-0">
            <div class="card-body p-5">
              <div class="text-center mb-5">
                <h1 class="fw-bold text-primary display-5 mb-1">EduShare</h1>
                <p class="text-muted">Postani deo zajednice i deli znanje</p>
              </div>
              
              <form @submit.prevent="handleRegister">
                <div class="row g-4">
                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Ime</label>
                    <input type="text" v-model="firstName" class="form-control custom-input" placeholder="Unesite ime" required @input="errorMessage = ''">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Prezime</label>
                    <input type="text" v-model="lastName" class="form-control custom-input" placeholder="Unesite prezime" required @input="errorMessage = ''">
                  </div>

                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Email adresa</label>
                    <input type="email" v-model="email" class="form-control custom-input" placeholder="ime@primer.com" required @input="errorMessage = ''">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Lozinka</label>
                    <input type="password" v-model="password" class="form-control custom-input" placeholder="Min. 8 karaktera" required @input="errorMessage = ''">
                  </div>

                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Smer studija</label>
                    <select v-model="departmentId" class="form-select custom-input">
                      <option :value="1">Komunikacione tehnologije</option>
                      <option :value="2">Savremene računarske tehnologije</option>
                    </select>
                  </div>
                  <div class="col-md-6">
                    <label class="form-label fw-bold text-secondary small">Vaša uloga</label>
                    <select v-model="roleId" class="form-select custom-input">
                      <option :value="3">Student</option>
                      <option :value="2">Profesor</option>
                    </select>
                  </div>
                </div>

                <div v-if="errorMessage" class="error-box mt-4 text-center">
                  {{ errorMessage }}
                </div>

                <div class="text-center mt-4">
                  <button type="submit" class="btn btn-primary btn-lg w-100 shadow register-btn" :disabled="isLoading">
                    {{ isLoading ? 'Procesiranje...' : 'Registruj se sada' }}
                  </button>
                  
                  <div class="mt-4">
                    <p class="text-muted small">
                      Već imaš nalog? 
                      <router-link to="/login" class="text-primary fw-bold text-decoration-none">Prijavi se ovde</router-link>
                    </p>
                  </div>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { register } from '../service/authservice.js'; 

const router = useRouter();
const firstName = ref('');
const lastName = ref('');
const email = ref('');
const password = ref('');
const departmentId = ref(1);
const roleId = ref(3);
const isLoading = ref(false);
const errorMessage = ref('');

const handleRegister = async () => {
  errorMessage.value = '';

  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailPattern.test(email.value)) {
    errorMessage.value = "Molimo unesite ispravan format email adrese.";
    return;
  }

  if (password.value.length < 8) {
    errorMessage.value = "Lozinka mora imati najmanje 8 karaktera.";
    return;
  }

  isLoading.value = true;
  try {
    const userData = {
      firstName: firstName.value,
      lastName: lastName.value,
      email: email.value,
      password: password.value,
      departmentId: departmentId.value,
      roleId: roleId.value
    };

    await register(userData);
    router.push('/login');
  } catch (error) {
    if (error.response && error.response.data) {
      errorMessage.value = error.response.data.message || "Podaci nisu ispravni.";
    } else {
      errorMessage.value = "Greška pri povezivanju sa serverom.";
    }
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  background: linear-gradient(135deg, #e0f2fe 0%, #f0f9ff 100%);
  position: relative;
  overflow: hidden;
}

.cloud {
  position: absolute;
  background: white;
  filter: blur(50px);
  border-radius: 50%;
  opacity: 0.6;
  z-index: 0;
  animation: float 20s infinite alternate ease-in-out;
}
.cloud-1 { width: 400px; height: 400px; top: -100px; left: -100px; }
.cloud-2 { width: 300px; height: 300px; bottom: -50px; right: -50px; animation-delay: -5s; }
.cloud-3 { width: 250px; height: 250px; top: 20%; right: 15%; animation-delay: -10s; }

@keyframes float {
  from { transform: translate(0, 0); }
  to { transform: translate(30px, 50px); }
}

.register-card {
  z-index: 1;
  border-radius: 30px;
  background: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(15px);
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.custom-input {
  border-radius: 12px;
  padding: 12px 20px;
  border: 1px solid #e2e8f0;
  background-color: #f8fafc;
  transition: all 0.3s ease;
}

.custom-input:focus {
  background-color: #fff;
  border-color: #0d6efd;
  box-shadow: 0 0 0 4px rgba(13, 110, 253, 0.1);
}

.register-btn {
  border-radius: 12px;
  font-weight: 700;
  padding: 12px;
  transition: transform 0.2s;
}

.register-btn:hover:not(:disabled) {
  transform: translateY(-2px);
}

.error-box {
  color: #dc3545;
  background: rgba(220, 53, 69, 0.1);
  padding: 10px;
  border-radius: 10px;
  font-weight: 600;
  font-size: 0.9rem;
}

.loading-overlay {
  position: fixed;
  inset: 0;
  background: rgba(255, 255, 255, 0.7);
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  z-index: 1000;
  backdrop-filter: blur(5px);
}

.blur-content {
  filter: blur(5px);
  pointer-events: none;
}

.animate-pulse {
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.5; }
}
</style>