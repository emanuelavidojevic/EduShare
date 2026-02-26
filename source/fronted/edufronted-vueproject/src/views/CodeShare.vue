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
            <p class="text-muted small mb-4">Unesi naziv i objavi svoj rad.</p>
            
            <form @submit.prevent="publishCode">
              <div class="mb-4">
                <label class="form-label small fw-bold text-uppercase text-primary">Naslov fajla</label>
                <input 
                  v-model="newEntry.filename" 
                  type="text" 
                  class="form-control form-control-lg rounded-3 bg-light border-0" 
                  placeholder="npr. MojeResenje">
              </div>

              <button type="submit" class="btn btn-primary btn-lg w-100 rounded-pill shadow-sm fw-bold py-3">
                <i class="bi bi-cloud-arrow-up-fill me-2"></i>Objavi
              </button>
            </form>

            <div class="mt-4 pt-4 border-top">
              <div class="d-flex justify-content-between text-muted small">
                <span>Ukupno objava:</span>
                <span class="fw-bold text-dark">{{ sharedCodes.length }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

    </div>

    <div class="row mt-5">
      <div class="col-12">
        <h5 class="fw-bold mb-4">Nedavne objave</h5>
        <div class="row row-cols-1 row-cols-md-2 row-cols-xl-3 g-4">
          <div v-for="(item, index) in sharedCodes" :key="index" class="col">
            <div class="card border-0 shadow-sm h-100 rounded-4">
              <div class="card-body p-4">
                <div class="d-flex justify-content-between align-items-start mb-3">
                  <h6 class="fw-bold m-0">{{ item.filename }}</h6>
                  <button @click="copyToClipboard(item.code)" class="btn btn-sm btn-light rounded-circle">
                    <i class="bi bi-clipboard"></i>
                  </button>
                </div>
                <pre class="bg-dark text-white p-3 rounded-3 small mb-3"><code>{{ item.code.substring(0, 100) }}...</code></pre>
                <div class="d-flex align-items-center">
                  <div class="avatar-xs bg-primary text-white rounded-circle me-2">{{ item.author[0] }}</div>
                  <span class="small text-muted">{{ item.author }} • {{ item.time }}</span>
                </div>
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

const sharedCodes = ref([
  {
    author: 'Lazar',
    filename: 'Primer',
    time: 'pre 2h',
    code: 'public void Start() {\n  Debug.Log("Sve radi!");\n}'
  }
]);

const newEntry = ref({ filename: '', code: '' });

const publishCode = () => {
  if (!newEntry.value.code) return;
  
  sharedCodes.value.unshift({
    author: 'Korisnik', 
    filename: newEntry.value.filename || 'Bez_Naslova',
    time: 'Upravo sada',
    code: newEntry.value.code
  });

  newEntry.value = { filename: '', code: '' };
};

const copyToClipboard = (text) => {
  navigator.clipboard.writeText(text);
  alert('Kod je kopiran!');
};
</script>

<style scoped>
/* Tackice u headeru */
.dot { width: 12px; height: 12px; border-radius: 50%; }
.red { background: #ff5f56; }
.yellow { background: #ffbd2e; }
.green { background: #27c93f; }

/* Editor Styling */
.editor-wrapper {
  background: #1e1e1e;
  padding: 10px;
}

.dark-textarea {
  width: 100%;
  height: 500px;
  background: #1e1e1e;
  color: #d4d4d4;
  border: none;
  padding: 20px;
  font-family: 'Fira Code', 'Courier New', monospace;
  font-size: 1rem;
  line-height: 1.6;
  resize: none;
  outline: none;
}

/* User Avatar */
.avatar-xs {
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  font-weight: bold;
}

/* Custom Scrollbar za editor */
.dark-textarea::-webkit-scrollbar { width: 8px; }
.dark-textarea::-webkit-scrollbar-thumb { background: #333; border-radius: 4px; }
.dark-textarea::-webkit-scrollbar-thumb:hover { background: #444; }
</style>