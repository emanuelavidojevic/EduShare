<template>
  <div class="container-fluid py-4 animate__animated animate__fadeIn">
    <div class="row g-4">
      <div class="col-lg-8">
        <div class="card border-0 shadow-lg rounded-4 overflow-hidden">
          <div class="card-header bg-dark d-flex justify-content-between align-items-center py-3 px-4">
            <div class="d-flex gap-2">
              <div class="dot red"></div>
              <div class="dot yellow"></div>
              <div class="dot green"></div>
            </div>
            <span class="text-white-50 small font-monospace">editor</span>
          </div>
          <div class="editor-wrapper">
            <textarea 
              v-model="newEntry.code" 
              class="dark-textarea" 
              placeholder="/* Nalepi svoj kod ovde i podeli ga sa kolegama... */" 
              spellcheck="false">
            </textarea>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="sticky-top" style="top: 20px;">
          <div class="card border-0 shadow-sm rounded-4 p-4 bg-white border-start border-primary border-4">
            <h4 class="fw-bold mb-1">CodeShare</h4>
            <p class="text-muted small mb-4">Unesi naziv i objavi svoj rad direktno u bazu.</p>
            
            <form @submit.prevent="publishCode">
              <div class="mb-4">
                <label class="form-label small fw-bold text-uppercase text-primary">Naslov fajla</label>
                <input 
                  v-model="newEntry.filename" 
                  type="text" 
                  class="form-control form-control-lg rounded-3 bg-light border-0" 
                  placeholder="npr. MojeResenje">
              </div>

              <button type="submit" class="btn btn-primary btn-lg w-100 rounded-pill shadow-sm fw-bold py-3" :disabled="loading">
                <span v-if="loading" class="spinner-border spinner-border-sm me-2"></span>
                <i v-else class="bi bi-cloud-arrow-up-fill me-2"></i>Objavi
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const newEntry = ref({ filename: '', code: '' });
const loading = ref(false);
const userId = ref(null);

const fetchUser = async () => {
  try {
    const res = await axios.get('http://localhost:5145/api/Auth/me', { withCredentials: true });
    userId.value = res.data.userId;
  } catch (e) {
    console.error("Korisnik nije ulogovan.");
  }
};

const publishCode = async () => {
  if (!newEntry.value.code || !userId.value) {
    alert("Morate uneti kod i biti ulogovani!");
    return;
  }

  try {
    loading.value = true;
    
    // Kreiranje virtuelnog .txt fajla iz teksta u editoru
    const blob = new Blob([newEntry.value.code], { type: 'text/plain' });
    const file = new File([blob], (newEntry.value.filename || 'Snippet') + '.txt', { type: 'text/plain' });
    
    const formData = new FormData();
    formData.append('File', file);
    formData.append('UserId', userId.value.toString());
    formData.append('SubjectId', "1");
    formData.append('Description', "Objavljeno putem CodeShare-a");

    await axios.post('http://localhost:5145/api/Material/upload', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
      withCredentials: true
    });

    // Čišćenje forme nakon uspeha
    newEntry.value = { filename: '', code: '' };
    alert("Uspešno sačuvano u bazi!");

  } catch (e) {
    alert("Greška pri slanju na server.");
  } finally {
    loading.value = false;
  }
};

onMounted(fetchUser);
</script>

<style scoped>
.dot { width: 12px; height: 12px; border-radius: 50%; }
.red { background: #ff5f56; }
.yellow { background: #ffbd2e; }
.green { background: #27c93f; }

.editor-wrapper { background: #1e1e1e; padding: 10px; }
.dark-textarea {
  width: 100%; 
  height: 600px; 
  background: #1e1e1e; 
  color: #d4d4d4;
  border: none; 
  padding: 25px; 
  font-family: 'Fira Code', monospace; 
  font-size: 1.1rem;
  line-height: 1.6;
  resize: none; 
  outline: none;
}

.dark-textarea::-webkit-scrollbar { width: 8px; }
.dark-textarea::-webkit-scrollbar-thumb { background: #333; border-radius: 4px; }
.dark-textarea::-webkit-scrollbar-thumb:hover { background: #444; }
</style>