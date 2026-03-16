<template>
  <div class="login-page">
    <div class="cloud cloud-1"></div>
    <div class="cloud cloud-2"></div>
    
    <div class="container">
      <div class="row justify-content-center w-100 m-0">
        <div class="col-md-5 col-lg-4">
          <div class="card login-card shadow-lg border-0">
            <div class="card-body p-5">
              <h1 class="fw-bold text-primary fs-2 mb-1 text-center d-block w-100">
                <i class="bi bi-mortarboard-fill me-2"></i>EduShare
              </h1>
              <p class="text-muted text-center mb-4 small">Prijava putem sigurne sesije</p>
              
              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label class="form-label text-secondary small fw-bold">Email adresa</label>
                  <input type="email" v-model="email" class="form-control custom-input" placeholder="ime@primer.com" required>
                </div>
                <div class="mb-4">
                  <label class="form-label text-secondary small fw-bold">Lozinka</label>
                  <input type="password" v-model="password" class="form-control custom-input" placeholder="••••••••" required>
                </div>
                
                <div v-if="errorMessage" class="error-box mb-3">{{ errorMessage }}</div>
                
                <button type="submit" class="btn btn-primary w-100 py-2 shadow" :disabled="isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                  {{ isLoading ? 'Prijava...' : 'Prijavi se' }}
                </button>

                <div class="text-center mt-3">
                  <p class="small text-muted">Nemate nalog? <router-link to="/register" class="text-primary fw-bold text-decoration-none">Registrujte se</router-link></p>
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
import { login } from '../service/authservice.js'; 

const email = ref('');
const password = ref('');
const isLoading = ref(false);
const errorMessage = ref('');
const router = useRouter();

const handleLogin = async () => {
  const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  if (!emailPattern.test(email.value)) {
    errorMessage.value = "Neispravan format emaila.";
    return;
  }
  
  isLoading.value = true;
  errorMessage.value = '';

  try {
    await login(email.value, password.value);
    router.push('/dashboard');
  } catch (error) {
    if (error.response && error.response.status === 401) {
        errorMessage.value = "Pogrešan email ili lozinka!";
    } else {
        errorMessage.value = "Server nije dostupan.";
    }
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.login-page { 
  position: fixed; 
  top: 0; left: 0; width: 100vw; height: 100vh;
  display: flex !important; align-items: center; justify-content: center;
  background: linear-gradient(135deg, #e0f2fe 0%, #f0f9ff 100%);
  z-index: 9999;
}
.login-card { min-width: 320px; border-radius: 24px; background: white; z-index: 10000; position: relative; }
.cloud { position: absolute; background: white; filter: blur(50px); border-radius: 50%; opacity: 0.6; z-index: 0; }
.cloud-1 { width: 400px; height: 400px; top: -100px; left: -100px; }
.cloud-2 { width: 300px; height: 300px; bottom: -50px; right: -50px; }
.custom-input { border-radius: 10px; padding: 10px 15px; background-color: #f8fafc; border: 1px solid #e2e8f0; }
.error-box { color: #dc3545; background: rgba(220, 53, 69, 0.1); padding: 8px; border-radius: 8px; text-align: center; font-weight: bold; font-size: 0.85rem; }
</style>