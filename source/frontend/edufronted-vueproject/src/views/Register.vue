<template>
  <div class="login-page">
    <div class="cloud cloud-1"></div>
    <div class="cloud cloud-2"></div>
    
    <div v-if="isLoading || isSuccess" class="loading-overlay">
       <div v-if="isLoading" class="spinner-border text-primary"></div>
       <div v-if="isSuccess" class="text-center">
          <i class="bi bi-check-circle-fill text-success fs-1"></i>
          <h5 class="mt-2 fw-bold text-success">Uspešno!</h5>
       </div>
    </div>

    <div class="container">
      <div class="row justify-content-center w-100 m-0">
        <div class="col-md-8 col-lg-6 col-xl-5"> 
          <div class="card login-card shadow-lg border-0">
            <div class="card-body p-4 p-md-5">
              <h1 class="fw-bold text-primary fs-2 mb-1 text-center d-block w-100">
                <i class="bi bi-mortarboard-fill me-2"></i>EduShare
              </h1>
              <p class="text-muted text-center mb-4 small">Kreirajte svoj nalog</p>
              
              <form @submit.prevent="handleRegister">
                <div class="row">
                  <div class="col-6 mb-3">
                    <label class="form-label text-secondary small fw-bold">Ime</label>
                    <input type="text" v-model="firstName" class="form-control custom-input" placeholder="Ime" required>
                  </div>
                  <div class="col-6 mb-3">
                    <label class="form-label text-secondary small fw-bold">Prezime</label>
                    <input type="text" v-model="lastName" class="form-control custom-input" placeholder="Prezime" required>
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label text-secondary small fw-bold">Email adresa</label>
                  <input type="email" v-model="email" class="form-control custom-input" placeholder="ime@primer.com" required>
                </div>

                <div class="mb-3">
                  <label class="form-label text-secondary small fw-bold">Lozinka</label>
                  <input type="password" v-model="password" class="form-control custom-input" placeholder="••••••••" required>
                </div>

                <div class="row">
                  <div class="col-6 mb-4">
                    <label class="form-label text-secondary small fw-bold">Smer</label>
                    <select v-model="departmentId" class="form-select custom-input">
                      <option :value="1">SRT</option>
                      <option :value="2">KOT</option>
                    </select>
                  </div>
                  <div class="col-6 mb-4">
                    <label class="form-label text-secondary small fw-bold">Uloga</label>
                    <select v-model="roleId" class="form-select custom-input">
                      <option :value="2">Student</option>
                      <option :value="3">Profesor</option>
                      <option :value="1">Admin</option>
                    </select>
                  </div>
                </div>
                
                <div v-if="errorMessage" class="error-box mb-3">{{ errorMessage }}</div>
                
                <button type="submit" class="btn btn-primary w-100 py-2 shadow" :disabled="isLoading || isSuccess">
                  {{ isLoading ? 'Registracija...' : 'Registruj se' }}
                </button>

                <div class="text-center mt-3">
                  <p class="small text-muted">Imate nalog? <router-link to="/login" class="text-primary fw-bold text-decoration-none">Prijavite se</router-link></p>
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

// Postavljamo SRT (1) i Student (2) kao podrazumevane vrednosti
const departmentId = ref(1); 
const roleId = ref(2);      

const isLoading = ref(false);
const isSuccess = ref(false);
const errorMessage = ref('');

const handleRegister = async () => {
  errorMessage.value = '';
  isLoading.value = true;
  try {
    const userData = {
      FirstName: firstName.value,
      LastName: lastName.value,
      Email: email.value,
      Password: password.value,
      DepartmentId: departmentId.value,
      RoleId: roleId.value
    };
    
    await register(userData);
    
    isLoading.value = false;
    isSuccess.value = true;
    
    // Preusmeravanje nakon uspeha
    setTimeout(() => { 
      router.push('/login'); 
    }, 1500);
    
  } catch (error) {
    isLoading.value = false;
    // Prikaz greške sa bekenda ili generička poruka
    errorMessage.value = error.response?.data?.Message || "Došlo je do greške prilikom registracije.";
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
.custom-input { border-radius: 10px; padding: 10px 15px; background-color: #f8fafc; border: 1px solid #e2e8f0; font-size: 0.85rem; }
.custom-input:focus { border-color: #0d6efd; box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.1); }
.error-box { color: #dc3545; background: rgba(220, 53, 69, 0.1); padding: 8px; border-radius: 8px; text-align: center; font-weight: bold; font-size: 0.85rem; }

.loading-overlay {
  position: absolute; inset: 0; background: rgba(255,255,255,0.8);
  display: flex; align-items: center; justify-content: center;
  z-index: 10001; border-radius: 24px;
}
</style>