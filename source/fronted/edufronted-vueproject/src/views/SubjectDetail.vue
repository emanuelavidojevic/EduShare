<template>
  <div class="subject-detail p-4">
    <div class="d-flex align-items-center mb-4">
      <button @click="goBack" class="back-btn shadow-sm me-3" title="Nazad na predmete">
        <i class="bi bi-arrow-left"></i>
      </button>

      <div>
        <div class="mb-1">
          <h2 class="subject-title d-inline-block">{{ subjectName }}</h2>
        </div>
        
        <nav aria-label="breadcrumb">
          <ol class="breadcrumb mb-0">
            <li class="breadcrumb-item">
               <router-link to="/subject" class="breadcrumb-link-box">Predmeti</router-link>
            </li>
            <li class="breadcrumb-item divider">/</li>
            <li class="breadcrumb-item active">{{ subjectName }}</li>
          </ol>
        </nav>
      </div>
    </div>

    <div class="row g-4">
      <div class="col-lg-8">
        <div class="card border-0 shadow-sm rounded-4 mb-4 overflow-hidden">
          <div class="card-header bg-white border-0 py-3 d-flex justify-content-between align-items-center">
            <h5 class="fw-bold mb-0">Materijali sa predavanja</h5>
            <button @click="showModal = true" class="btn btn-primary btn-sm rounded-pill px-3 shadow-sm">
              <i class="bi bi-upload me-2"></i>Dodaj fajl
            </button>
          </div>
          <div class="card-body p-0">
            <div class="table-responsive">
              <table class="table table-hover align-middle mb-0">
                <thead class="bg-light">
                  <tr>
                    <th class="ps-4">Naziv fajla</th>
                    <th>Tip</th>
                    <th>Datum</th>
                    <th class="text-end pe-4">Akcija</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(file, index) in materials" :key="index">
                    <td class="ps-4">
                      <div class="d-flex align-items-center">
                        <i :class="['bi fs-4 me-3', getFileIcon(file.type)]"></i>
                        <span class="fw-medium text-dark">{{ file.name }}</span>
                      </div>
                    </td>
                    <td><span class="badge bg-light text-dark border fw-normal">{{ file.type }}</span></td>
                    <td class="text-muted small">{{ file.date }}</td>
                    <td class="text-end pe-4">
                      <button class="btn btn-sm btn-light border me-2" title="Preuzmi"><i class="bi bi-download"></i></button>
                      <button class="btn btn-sm btn-outline-danger border-0" @click="removeFile(index)"><i class="bi bi-trash"></i></button>
                    </td>
                  </tr>
                  <tr v-if="materials.length === 0">
                    <td colspan="4" class="text-center py-5 text-muted">
                      <i class="bi bi-folder2-open display-4 d-block mb-2"></i>
                      Još uvek nema materijala za ovaj predmet.
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>

      <div class="col-lg-4">
        <div class="card border-0 shadow-sm rounded-4 p-4 mb-4 bg-primary text-white">
          <h5 class="fw-bold mb-3">O predmetu</h5>
          <p class="small opacity-75">
            Svi materijali su dostupni za preuzimanje svim studentima. Ukoliko imaš koristan fajl, slobodno ga podeli sa kolegama.
          </p>
          <hr class="border-white opacity-25">
          <div class="d-flex justify-content-between small">
            <span>Broj resursa:</span>
            <span class="fw-bold">{{ materials.length }}</span>
          </div>
        </div>
      </div>
    </div>

    <div v-if="showModal" class="custom-modal-overlay">
      <div class="custom-modal-card">
        <div class="d-flex justify-content-between align-items-center mb-4">
          <h4 class="fw-bold mb-0">Dodaj fajl</h4>
          <button @click="showModal = false" class="btn-close"></button>
        </div>
        
        <form @submit.prevent="addNewFile">
          <div class="mb-3">
            <label class="form-label small fw-bold text-muted">Naziv dokumenta</label>
            <input v-model="newFile.name" type="text" class="form-control" placeholder="Unesite naziv..." required>
          </div>
          <div class="mb-4">
            <label class="form-label small fw-bold text-muted">Tip</label>
            <select v-model="newFile.type" class="form-select">
              <option value="PDF">PDF</option>
              <option value="ZIP">ZIP</option>
              <option value="DOCX">Word</option>
            </select>
          </div>
          <div class="d-flex gap-2">
            <button type="button" @click="showModal = false" class="btn btn-light w-100 rounded-pill">Odustani</button>
            <button type="submit" class="btn btn-primary w-100 rounded-pill shadow-sm">Potvrdi</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const route = useRoute();
const router = useRouter();

// Rešavanje TypeScript greške i formatiranje naslova
const subjectName = computed(() => {
  const idParam = route.params.id;
  const id = Array.isArray(idParam) ? idParam[0] : idParam;
  if (!id) return 'Predmet';
  // Menja crtice u razmake i svaku reč počinje velikim slovom (npr. c-sharp -> C Sharp)
  return id.split('-').map(word => word.charAt(0).toUpperCase() + word.slice(1)).join(' ');
});

const goBack = () => router.push('/subject');

const showModal = ref(false);
const materials = ref([
  { name: 'Predavanje 1 - Uvod.pdf', type: 'PDF', date: '15. Feb 2026' },
  { name: 'Vezba_02_Resenja.zip', type: 'ZIP', date: '17. Feb 2026' }
]);

const newFile = ref({ name: '', type: 'PDF' });

const addNewFile = () => {
  const today = new Date().toLocaleDateString('sr-RS', { day: '2-digit', month: 'short', year: 'numeric' });
  materials.value.push({
    name: newFile.value.name + (newFile.value.type === 'PDF' ? '.pdf' : newFile.value.type === 'ZIP' ? '.zip' : '.docx'),
    type: newFile.value.type,
    date: today
  });
  newFile.value = { name: '', type: 'PDF' };
  showModal.value = false;
};

const removeFile = (index: number) => {
  materials.value.splice(index, 1);
};

const getFileIcon = (type: string) => {
  switch(type) {
    case 'PDF': return 'bi-file-earmark-pdf text-danger';
    case 'ZIP': return 'bi-file-earmark-zip text-warning';
    case 'DOCX': return 'bi-file-earmark-word text-primary';
    default: return 'bi-file-earmark-text';
  }
};
</script>

<style scoped>
.subject-detail { animation: fadeIn 0.4s ease-out; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(10px); } to { opacity: 1; transform: translateY(0); } }

/* STILIZACIJA HEADERA SA SLIKE */
.back-btn {
  width: 42px;
  height: 42px;
  border-radius: 50%;
  border: 1px solid #eee;
  background: white;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s;
}
.back-btn:hover { background: #f8f9fa; transform: scale(1.05); }

.subject-title {
  background-color: #3b71db; /* Karakteristična plava sa tvoje slike */
  color: white;
  padding: 4px 12px;
  font-weight: 700;
  margin: 0;
  border-radius: 2px;
}

.breadcrumb-link-box {
  background-color: #3b71db;
  color: white !important;
  padding: 2px 8px;
  text-decoration: none;
  font-size: 0.9rem;
  border-radius: 2px;
}

.divider {
  padding: 0 8px;
  color: #adb5bd;
  font-weight: 300;
}

.breadcrumb-item.active {
  color: #495057;
  font-weight: 500;
  font-size: 0.9rem;
}

.breadcrumb-item + .breadcrumb-item::before { content: none; }

/* TABELA I KARTICE */
.rounded-4 { border-radius: 1rem !important; }
.table th { font-size: 0.75rem; text-transform: uppercase; letter-spacing: 0.8px; color: #adb5bd; padding: 1rem 0.5rem; }

/* MODAL */
.custom-modal-overlay {
  position: fixed;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0, 0, 0, 0.4);
  backdrop-filter: blur(6px);
  display: flex; align-items: center; justify-content: center;
  z-index: 2000;
}
.custom-modal-card {
  background: white;
  padding: 2rem;
  border-radius: 20px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 15px 30px rgba(0,0,0,0.1);
}
</style>