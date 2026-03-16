<template>
  <div class="admin-panel p-4 bg-light min-vh-100">
    <div class="d-flex align-items-center mb-4">
      <div class="bg-primary text-white rounded-circle d-flex align-items-center justify-content-center shadow-sm me-3" style="width: 48px; height: 48px;">
        <i class="bi bi-shield-lock-fill fs-4"></i>
      </div>
      <div>
        <h2 class="fw-bold mb-0 text-dark">Upravljanje sistemom</h2>
        <p class="text-muted small mb-0">Administratorski panel za upravljanje korisnicima i resursima</p>
      </div>
    </div>

    <div class="row g-4 mb-4">
      <div class="col-md-4">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white h-100 stat-card-hover">
          <div class="d-flex align-items-center">
            <div class="icon-box bg-primary-subtle text-primary rounded-4 me-3">
              <i class="bi bi-people-fill fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted small fw-bold text-uppercase mb-1">Korisnici</h6>
              <h3 class="fw-extrabold mb-0">{{ stats.TotalUsers }}</h3>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white h-100 stat-card-hover">
          <div class="d-flex align-items-center">
            <div class="icon-box bg-success-subtle text-success rounded-4 me-3">
              <i class="bi bi-file-earmark-text-fill fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted small fw-bold text-uppercase mb-1">Materijali</h6>
              <h3 class="fw-extrabold mb-0">{{ stats.TotalMaterials }}</h3>
            </div>
          </div>
        </div>
      </div>
      <div class="col-md-4">
        <div class="card border-0 shadow-sm rounded-4 p-3 bg-white h-100 stat-card-hover">
          <div class="d-flex align-items-center">
            <div class="icon-box bg-warning-subtle text-warning rounded-4 me-3">
              <i class="bi bi-journal-bookmark-fill fs-3"></i>
            </div>
            <div>
              <h6 class="text-muted small fw-bold text-uppercase mb-1">Predmeti</h6>
              <h3 class="fw-extrabold mb-0">{{ stats.TotalSubjects }}</h3>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="card border-0 shadow-sm rounded-4 overflow-hidden bg-white">
      <div class="card-header bg-white border-0 py-4 px-4">
        <div class="d-flex justify-content-between align-items-center flex-wrap gap-3">
          <h5 class="fw-bold mb-0 text-dark">Lista svih korisnika</h5>
          <div class="d-flex gap-3 align-items-center">
            <div class="input-group input-group-sm border rounded-pill bg-light px-2" style="width: 280px;">
              <span class="input-group-text bg-transparent border-0"><i class="bi bi-search text-muted"></i></span>
              <input v-model="searchQuery" @input="handleSearch" type="text" class="form-control bg-transparent border-0 shadow-none py-2" placeholder="Pretraži korisnike...">
            </div>
            <select v-model="selectedRoleId" @change="fetchData" class="form-select form-select-sm rounded-pill border-light-subtle bg-light px-3 shadow-none" style="width: 150px;">
              <option :value="null">Sve uloge</option>
              <option :value="1">Admin</option>
              <option :value="2">Student</option>
              <option :value="3">Professor</option>
            </select>
          </div>
        </div>
      </div>

      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead class="bg-light text-uppercase small fw-bold text-muted">
              <tr>
                <th class="ps-4 py-3 border-0">Korisnik</th>
                <th class="border-0">Email adresa</th>
                <th class="border-0">Uloga</th>
                <th class="border-0">Smer</th>
                <th class="border-0 text-end pe-4">Opcije</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="user in users" :key="user.userId">
                <td class="ps-4 py-3">
                  <div class="d-flex align-items-center">
                    <div class="avatar-sm me-3 bg-primary-subtle text-primary rounded-circle d-flex align-items-center justify-content-center fw-bold shadow-sm" style="width: 38px; height: 38px;">
                      {{ user.fullName?.split(' ').map(n => n[0]).join('').toUpperCase() || '?' }}
                    </div>
                    <div v-if="editingId === user.userId" class="flex-grow-1">
                       <input v-model="editForm.fullName" class="form-control form-control-sm rounded-pill border-primary shadow-none">
                    </div>
                    <span v-else class="fw-bold text-dark">{{ user.fullName }}</span>
                  </div>
                </td>
                <td>
                  <input v-if="editingId === user.userId" v-model="editForm.email" class="form-control form-control-sm rounded-pill border-primary shadow-none">
                  <span v-else class="text-muted small">{{ user.email }}</span>
                </td>
                <td>
                  <select v-if="editingId === user.userId" v-model="editForm.roleName" class="form-select form-select-sm rounded-pill border-primary shadow-none py-0">
                    <option value="Admin">Admin</option>
                    <option value="Student">Student</option>
                    <option value="Professor">Professor</option>
                  </select>
                  <span v-else :class="['badge rounded-pill px-3 py-2 fw-bold', getRoleBadgeClass(user.roleName)]">
                    {{ user.roleName }}
                  </span>
                </td>
                <td class="text-muted small">{{ user.departmentName || 'Opšti' }}</td>
                <td class="text-end pe-4">
                  <div class="btn-group">
                    <template v-if="editingId === user.userId">
                      <button @click="saveEdit(user.userId)" class="btn btn-sm btn-success rounded-circle me-2 shadow-sm" style="width: 32px; height: 32px;">
                        <i class="bi bi-check-lg"></i>
                      </button>
                      <button @click="editingId = null" class="btn btn-sm btn-light border rounded-circle shadow-sm" style="width: 32px; height: 32px;">
                        <i class="bi bi-x-lg"></i>
                      </button>
                    </template>
                    <template v-else>
                      <button @click="startEdit(user)" class="btn btn-sm btn-outline-warning rounded-pill px-3 me-2 border-warning-subtle fw-medium">
                        <i class="bi bi-pencil-square"></i>
                      </button>
                      <button @click="openDeleteModal(user)" class="btn btn-sm btn-outline-danger rounded-pill px-3 border-danger-subtle fw-medium">
                        <i class="bi bi-trash3"></i>
                      </button>
                    </template>
                  </div>
                </td>
              </tr>
              <tr v-if="users.length === 0">
                <td colspan="5" class="text-center py-5 text-muted small italic">Nema pronađenih korisnika za ovaj kriterijum.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div v-if="deleteModal.show" class="custom-overlay">
      <div class="custom-modal shadow-lg border-danger-subtle border">
        <div class="icon-circle bg-danger-subtle">
          <i class="bi bi-trash3-fill text-danger"></i>
        </div>
        <h3 class="fw-bold text-dark mb-2">Brisanje korisnika</h3>
        <p class="text-muted mb-4 px-4">
          Da li ste sigurni da želite da obrišete korisnika <br>
          <span class="fw-bold text-dark">{{ deleteModal.userName }}</span>? <br>
          Ova akcija je trajna i ne može se poništiti.
        </p>
        <div class="d-flex gap-3 justify-content-center px-4 mb-2">
          <button @click="deleteModal.show = false" class="btn btn-light px-4 py-2 rounded-pill fw-bold border w-50">Otkaži</button>
          <button @click="confirmDelete" class="btn btn-danger px-4 py-2 rounded-pill fw-bold shadow-sm w-50">Izbriši</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';

const stats = ref({ TotalUsers: 0, TotalMaterials: 0, TotalSubjects: 0 });
const users = ref([]);
const searchQuery = ref("");
const selectedRoleId = ref(null);
const editingId = ref(null);
const editForm = ref({ fullName: '', email: '', roleName: '' });

// Modal state
const deleteModal = ref({ show: false, userId: null, userName: '' });

let searchTimeout = null;

const fetchData = async () => {
  try {
    const [uRes, sRes] = await Promise.all([
      axios.get('http://localhost:5145/api/Admin/users', { 
        params: { 
          search: searchQuery.value || null, 
          roleId: selectedRoleId.value || null 
        } 
      }),
      axios.get('http://localhost:5145/api/Admin/stats')
    ]);
    users.value = uRes.data;
    stats.value = sRes.data;
  } catch (err) {
    console.error("Greška:", err);
  }
};

const handleSearch = () => {
  clearTimeout(searchTimeout);
  searchTimeout = setTimeout(fetchData, 300);
};

const startEdit = (user) => {
  editingId.value = user.userId;
  editForm.value = { ...user };
};

const saveEdit = async (id) => {
  try {
    await axios.put(`http://localhost:5145/api/Admin/users/${id}`, editForm.value);
    editingId.value = null;
    fetchData();
  } catch (e) { alert("Greška pri čuvanju!"); }
};

const openDeleteModal = (user) => {
  deleteModal.value = { show: true, userId: user.userId, userName: user.fullName };
};

const confirmDelete = async () => {
  try {
    await axios.delete(`http://localhost:5145/api/Admin/users/${deleteModal.value.userId}`);
    deleteModal.value.show = false;
    fetchData();
  } catch (e) { alert("Greška pri brisanju!"); }
};

const getRoleBadgeClass = (role) => {
  const r = role?.toLowerCase();
  if (r === 'admin') return 'bg-danger-subtle text-danger';
  if (r === 'student') return 'bg-info-subtle text-info';
  if (r === 'professor' || r === 'profesor') return 'bg-warning-subtle text-warning';
  return 'bg-secondary-subtle text-secondary';
};

onMounted(fetchData);
</script>

<style scoped>
.admin-panel { font-family: 'Inter', sans-serif; }
.stat-card-hover { transition: transform 0.2s ease; }
.stat-card-hover:hover { transform: translateY(-4px); box-shadow: 0 10px 20px rgba(0,0,0,0.05) !important; }
.icon-box { width: 55px; height: 55px; display: flex; align-items: center; justify-content: center; }
.fw-extrabold { font-weight: 800; }
.table thead th { background-color: #f8fafc; font-size: 0.75rem; letter-spacing: 0.05em; }

/* Modal Dizajn iz SubjectDetail-a */
.custom-overlay {
  position: fixed; inset: 0; background: rgba(13, 110, 253, 0.15);
  backdrop-filter: blur(8px); display: flex; align-items: center; justify-content: center; z-index: 9999;
}
.custom-modal {
  background: white; padding: 2.5rem 1.5rem; border-radius: 35px;
  width: 90%; max-width: 420px; text-align: center;
  animation: modalAppear 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.15);
}
.icon-circle {
  width: 70px; height: 70px; border-radius: 50%; display: flex;
  align-items: center; justify-content: center; margin: 0 auto 1.5rem; font-size: 2.2rem;
}
@keyframes modalAppear { from { transform: scale(0.9); opacity: 0; } to { transform: scale(1); opacity: 1; } }
</style>