<template>
  <div class="container-fluid py-2">
    
    <div class="d-flex align-items-center mb-4 pt-3">
      <div class="position-relative me-3">
        <img src="https://ui-avatars.com/api/?name=Milica+User&background=0d6efd&color=fff" 
             alt="User Logo" 
             class="rounded-circle shadow-sm" 
             style="width: 65px; height: 65px; border: 3px solid white;">
        <span class="position-absolute bottom-0 end-0 badge border border-light rounded-circle bg-success p-2" style="transform: translate(-5%, -5%);">
          <span class="visually-hidden">Online</span>
        </span>
      </div>
      <div>
        <h3 class="fw-bold text-dark m-0">Zdravo, Emanuela! 👋</h3>
        <p class="text-muted m-0">Lepo je videti te ponovo na EduShare platformi.</p>
      </div>
    </div>

    <div class="mb-5">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h5 class="fw-bold m-0 text-secondary">Predstojeće obaveze</h5>
      </div>
      <div class="timeline-wrapper">
        <div class="timeline-scroll">
          <div class="timeline-card urgent">
            <div class="date-tag">Sutra</div>
            <h6 class="fw-bold mb-1 text-dark">Matematika 1: Kolokvijum</h6>
            <p class="small text-muted mb-0"><i class="bi bi-clock me-1"></i>09:00h - Sala 102</p>
          </div>
          <div class="timeline-card">
            <div class="date-tag text-primary">Petak</div>
            <h6 class="fw-bold mb-1 text-dark">CodeShare projekat</h6>
            <p class="small text-muted mb-0"><i class="bi bi-clock me-1"></i>Do ponoći</p>
          </div>
          <div class="timeline-card">
            <div class="date-tag text-success">Ponedeljak</div>
            <h6 class="fw-bold mb-1 text-dark">Prezentacija: Marketing</h6>
            <p class="small text-muted mb-0"><i class="bi bi-geo-alt me-1"></i>Kabinet 4</p>
          </div>
          <div class="timeline-card text-center d-flex align-items-center justify-content-center border-dashed">
            <span class="text-muted small">+ Dodaj podsetnik</span>
          </div>
        </div>
      </div>
    </div>

    <h3 class="fw-bold text-dark mb-4">Pregled aktivnosti</h3>

    <div class="row g-4 mb-5">
      <div class="col-md-4">
        <div class="card stat-card border-0 shadow-sm bg-primary text-white h-100">
          <div class="card-body p-4 d-flex align-items-center">
            <div class="stat-icon-wrapper me-3"><i class="bi bi-code-slash fs-2"></i></div>
            <div>
              <h2 class="fw-bold mb-0">32</h2>
              <p class="mb-0 opacity-75">Code Snippets</p>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card stat-card border-0 shadow-sm bg-warning text-white h-100">
          <div class="card-body p-4 d-flex align-items-center">
            <div class="stat-icon-wrapper me-3"><i class="bi bi-file-earmark-pdf fs-2"></i></div>
            <div>
              <h2 class="fw-bold mb-0">18</h2>
              <p class="mb-0 opacity-75">PDF Documents</p>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card stat-card border-0 shadow-sm bg-success text-white h-100">
          <div class="card-body p-4 d-flex align-items-center">
            <div class="stat-icon-wrapper me-3"><i class="bi bi-share fs-2"></i></div>
            <div>
              <h2 class="fw-bold mb-0">12</h2>
              <p class="mb-0 opacity-75">Tutorials Shared</p>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm bg-white rounded-4 overflow-hidden">
      <div class="card-header bg-white border-0 py-3 px-4">
        <h5 class="fw-bold mb-0 text-secondary">Najskorije dodati materijali</h5>
      </div>
      <div class="table-responsive px-4 pb-4">
        <table class="table align-middle">
          <thead class="table-light">
            <tr>
              <th class="border-0 text-muted small text-uppercase">Title</th>
              <th class="border-0 text-muted small text-uppercase">Type</th>
              <th class="border-0 text-muted small text-uppercase">Author</th>
              <th class="border-0 text-muted small text-uppercase">Date</th>
              <th class="border-0 text-muted small text-uppercase text-end">Actions</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in recentMaterials" :key="item.title">
              <td class="fw-bold text-primary">{{ item.title }}</td>
              <td><span class="badge bg-light text-dark border">{{ item.type }}</span></td>
              <td>{{ item.author }}</td>
              <td class="text-muted">{{ item.date }}</td>
              <td class="text-end">
                <div class="btn-group">
                  <button class="btn btn-sm btn-outline-primary border-0"><i class="bi bi-download"></i></button>
                  <button class="btn btn-sm btn-outline-secondary border-0"><i class="bi bi-pencil"></i></button>
                  <button class="btn btn-sm btn-outline-danger border-0"><i class="bi bi-trash"></i></button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>

<script setup>
const recentMaterials = [
  { title: 'JavaScript Async Guide', type: 'Code', author: 'Prof. Marković', date: '15 Feb 2024' },
  { title: 'Linear Algebra Notes', type: 'PDF', author: 'Dr. Petrović', date: '14 Feb 2024' },
  { title: 'React Basics Tutorial', type: 'Tutorial', author: 'Ana Student', date: '13 Feb 2024' },
  { title: 'Python OOP Examples', type: 'Code', author: 'Prof. Ilić', date: '12 Feb 2024' },
  { title: 'Quantum Mechanics Intro', type: 'PDF', author: 'Dr. Nikolić', date: '11 Feb 2024' }
];
</script>

<style scoped>
/* Pozdrav i Status */
.bg-success { background-color: #198754 !important; }

/* Timeline Stilovi */
.timeline-wrapper {
  overflow-x: auto;
  scrollbar-width: none; /* Firefox */
  padding-bottom: 10px;
}
.timeline-wrapper::-webkit-scrollbar { display: none; } /* Chrome/Safari */

.timeline-scroll {
  display: flex;
  gap: 1.2rem;
}

.timeline-card {
  min-width: 250px;
  background: white;
  padding: 1.2rem;
  border-radius: 14px;
  box-shadow: 0 4px 10px rgba(0,0,0,0.03);
  border-top: 4px solid #dee2e6;
}

.timeline-card.urgent { border-top-color: #dc3545; }
.border-dashed { border: 2px dashed #dee2e6; background: transparent; box-shadow: none; cursor: pointer; }

.date-tag { font-size: 0.75rem; font-weight: 800; text-transform: uppercase; margin-bottom: 5px; }

/* Tvoji originalni stilovi */
.stat-card {
  border-radius: 16px;
  transition: transform 0.3s;
}
.stat-card:hover { transform: translateY(-5px); }

.stat-icon-wrapper {
  background: rgba(255, 255, 255, 0.2);
  width: 55px;
  height: 55px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 12px;
}

.table thead th { font-weight: 600; padding: 15px 10px; }
.table tbody td { padding: 15px 10px; border-bottom: 1px solid #f8f9fa; }
.bg-warning { background-color: #ff9800 !important; }
</style>