<template>
  <div class="subjects-page">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="fw-bold text-dark">Moji Predmeti</h2>
      <button class="btn btn-primary shadow-sm rounded-pill px-4">
        <i class="bi bi-plus-lg me-2"></i>Dodaj novi predmet
      </button>
    </div>

    <div class="row g-4">
      <div v-for="subject in subjects" :key="subject.id" class="col-md-6 col-lg-4">
        <div class="card h-100 subject-card shadow-sm border-0" @click="goToSubject(subject.id)">
          <div class="card-body text-center p-4">
            <div :class="['icon-box mb-3', subject.bgClass]">
              <i :class="['bi', subject.icon, subject.textClass, 'fs-1']"></i>
            </div>
            <h5 class="fw-bold">{{ subject.name }}</h5>
            <p class="text-muted small">{{ subject.description }}</p>
            
            <div class="d-flex justify-content-center gap-2 mt-3">
              <button class="btn btn-sm btn-outline-danger border-0" @click.stop="deleteSubject(subject.name)">
                <i class="bi bi-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router';

const router = useRouter();

// Lista svih 6 predmeta sa tvojim podacima
const subjects = [
  { 
    id: 'c-sharp', 
    name: 'C#', 
    description: 'Programiranje i .NET framework', 
    icon: 'bi-hash', 
    bgClass: 'bg-purple-light', 
    textClass: 'text-purple' 
  },
  { 
    id: 'c-lang', 
    name: 'C', 
    description: 'Strukture podataka i algoritmi', 
    icon: 'bi-c-circle', 
    bgClass: 'bg-blue-light', 
    textClass: 'text-blue' 
  },
  { 
    id: 'database', 
    name: 'Baze Podataka', 
    description: 'SQL, Relacione baze i modelovanje', 
    icon: 'bi-database', 
    bgClass: 'bg-orange-light', 
    textClass: 'text-orange' 
  },
  { 
    id: 'python', 
    name: 'Python', 
    description: 'Skriptni jezici i Data Science', 
    icon: 'bi-filetype-py', 
    bgClass: 'bg-yellow-light', 
    textClass: 'text-warning' 
  },
  { 
    id: 'internet-tech', 
    name: 'Internet tehnologije', 
    description: 'Web razvoj, HTML, CSS i JS', 
    icon: 'bi-globe', 
    bgClass: 'bg-green-light', 
    textClass: 'text-success' 
  },
  { 
    id: 'client-server', 
    name: 'Klijent server sistemi', 
    description: 'Distribuirani sistemi i API', 
    icon: 'bi-server', 
    bgClass: 'bg-info-light', 
    textClass: 'text-info' 
  }
];

const goToSubject = (id) => {
  router.push(`/dashboard/subject/${id}`);
};

const deleteSubject = (name) => {
  alert(`Da li ste sigurni da želite da obrišete predmet: ${name}?`);
};
</script>

<style scoped>
.subject-card {
  transition: all 0.3s ease;
  cursor: pointer;
  border-radius: 20px;
}

.subject-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 15px 30px rgba(0,0,0,0.1) !important;
}

.icon-box {
  width: 85px;
  height: 85px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 22px;
}

/* Boje za ikonice i pozadine */
.bg-purple-light { background-color: #f3e5f5; }
.text-purple { color: #9c27b0; }

.bg-blue-light { background-color: #e3f2fd; }
.text-blue { color: #2196f3; }

.bg-orange-light { background-color: #fff3e0; }
.text-orange { color: #ff9800; }

.bg-yellow-light { background-color: #fffde7; }

.bg-green-light { background-color: #e8f5e9; }

.bg-info-light { background-color: #e0f7fa; }

/* Suptilna kanta koja se vidi bolje na hover */
.btn-outline-danger {
  opacity: 0.5;
  transition: opacity 0.2s;
}
.subject-card:hover .btn-outline-danger {
  opacity: 1;
}
</style>