<template>
  <div class="login-page">
    <div class="cloud cloud-1"></div>
    <div class="cloud cloud-2"></div>
    <div class="cloud cloud-3"></div>
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-md-5 col-lg-4">
          <div class="card login-card shadow-lg border-0">
            <div class="card-body p-5">
             <h1 class="fw-bold text-primary display-5 mb-1">
    <i class="bi bi-mortarboard-fill me-2"></i>EduShare
  </h1>
              <p class="text-muted text-center mb-4 small">Dobrodošli nazad!</p>
              <h3 class="text-center mb-4 fw-bold">Prijava</h3>
              <form @submit.prevent="handleLogin">
                <div class="mb-3">
                  <label class="form-label text-secondary small fw-bold">Email adresa</label>
                  <input type="email" v-model="email" @input="errorMessage = ''" class="form-control custom-input" placeholder="ime@primer.com" required>
                </div>
                <div class="mb-4">
                  <label class="form-label text-secondary small fw-bold">Lozinka</label>
                  <input type="password" v-model="password" @input="errorMessage = ''" class="form-control custom-input" placeholder="••••••••" required>
                </div>
                <div v-if="errorMessage" class="error-box mb-3">{{ errorMessage }}</div>
                <button type="submit" class="btn btn-primary w-100 py-2 shadow login-btn" :disabled="!email || !password || isLoading">
                  <span v-if="isLoading" class="spinner-border spinner-border-sm me-2"></span>
                  {{ isLoading ? 'Prijava...' : 'Prijavi se' }}
                </button>
              </form>
              <div class="mt-4 text-center">
                <small class="text-muted">Nemaš nalog? <router-link to="/register" class="text-primary fw-bold text-decoration-none">Registruj se</router-link></small>
              </div>
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
    const userData = await login(email.value, password.value);
    
    if (userData) {
        // Ključni deo: Skladištimo podatke odmah
        localStorage.setItem('user_display', JSON.stringify({
            firstName: userData.firstName,
            lastName: userData.lastName
        }));
    }

    router.push('/dashboard');
  } catch (error) {
    errorMessage.value = "Pogrešan email ili lozinka!";
  } finally {
    isLoading.value = false;
  }
};
</script>

<style scoped>
.login-page { min-height: 100vh; display: flex; align-items: center; background: linear-gradient(135deg, #e0f2fe 0%, #f0f9ff 100%); position: relative; overflow: hidden; }
.cloud { position: absolute; background: white; filter: blur(50px); border-radius: 50%; opacity: 0.6; z-index: 0; animation: float 20s infinite alternate ease-in-out; }
.cloud-1 { width: 400px; height: 400px; top: -100px; left: -100px; }
.cloud-2 { width: 300px; height: 300px; bottom: -50px; right: -50px; }
.cloud-3 { width: 250px; height: 250px; top: 20%; right: 15%; }
@keyframes float { from { transform: translate(0, 0); } to { transform: translate(30px, 50px); } }
.login-card { z-index: 1; border-radius: 24px; background: rgba(255, 255, 255, 0.9); backdrop-filter: blur(10px); }
.custom-input { border-radius: 10px; padding: 10px 15px; border: 1px solid #e2e8f0; background-color: #f8fafc; }
.login-btn { border-radius: 10px; font-weight: 600; }
.error-box { color: #dc3545; background: rgba(220, 53, 69, 0.1); padding: 8px; border-radius: 8px; font-size: 0.85rem; text-align: center; font-weight: bold; }
</style>