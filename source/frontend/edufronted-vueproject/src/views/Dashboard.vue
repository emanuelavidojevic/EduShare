<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useRouter, useRoute } from 'vue-router'; 
import api from '../service/api'; 
import { useAuthStore } from '../stores/auth.js';
import Session from '@/views/Session.vue';

const router = useRouter();
const route = useRoute(); 
const authStore = useAuthStore();

const userName = ref('');
const userLastName = ref('');
const userRole = ref(''); 
const showSession = ref(false);
const notifications = ref([]);
const isDataLoaded = ref(false); 
let expiryTimer = null;
let notificationTimer = null;

// Spajanje imena i prezimena za prikaz pored avatara
const displayName = computed(() => {
  return `${userName.value} ${userLastName.value}`;
});

// POPRAVLJENI INICIJALI: Ime + Prezime (podržava "prof." prefiks)
const initials = computed(() => {
  let f = userName.value ? userName.value.trim() : '';
  const l = userLastName.value ? userLastName.value.trim() : '';
  
  // Ako ime počinje sa "prof.", uzimamo slovo pravog imena koje sledi nakon toga
  if (f.toLowerCase().startsWith('prof.')) {
    const parts = f.split(' ');
    f = parts[1] || ''; 
  }
  
  const firstLetter = f[0] || '';
  const lastLetter = l[0] || '';
  
  const res = (firstLetter + lastLetter).toUpperCase();
  
  return res || '??';
});

const isAdmin = computed(() => {
  const role = userRole.value || authStore.user?.role;
  return role === 'Admin';
});

const unreadCount = computed(() => {
  if (!notifications.value) return 0;
  return notifications.value.filter(n => !(n.isRead || n.IsRead)).length;
});

const startTimer = (expiryTime) => {
  if (expiryTimer) clearTimeout(expiryTimer);
  const timeLeft = new Date(expiryTime).getTime() - new Date().getTime();
  if (timeLeft > 30000) {
    expiryTimer = setTimeout(() => { showSession.value = true; }, timeLeft - 30000);
  }
};

const fetchNotifications = async () => {
  try {
    const res = await api.get(`Notifications?t=${Date.now()}`);
    notifications.value = res.data || [];
  } catch (e) { console.error("Greška pri osvežavanju notifikacija"); }
};

const handleRefresh = async () => {
  try {
    const res = await api.post('Auth/refresh-access-token');
    showSession.value = false;
    if (res.data?.expiry) startTimer(res.data.expiry);
  } catch (err) { handleLogout(); }
};

const loadUserData = async () => {
  try {
    const res = await api.get('Auth/me');
    if (res.data) {
      userName.value = res.data.firstName;
      userLastName.value = res.data.lastName;
      userRole.value = res.data.role;
      
      authStore.setUser(res.data);
      if (res.data.expiry) startTimer(res.data.expiry);
      isDataLoaded.value = true; 
    }
  } catch (e) { 
    handleLogout(); 
  }
};

const handleLogout = async () => {
  try { await api.post('Auth/logout'); } finally {
    authStore.logout();
    router.push('/login');
  }
};

onMounted(() => {
  loadUserData();
  fetchNotifications();
  notificationTimer = setInterval(fetchNotifications, 15000);
});

onUnmounted(() => { 
  if (expiryTimer) clearTimeout(expiryTimer); 
  if (notificationTimer) clearInterval(notificationTimer);
});
</script>

<template>
  <div v-if="isDataLoaded" class="dashboard-wrapper">
    <aside id="sidebar-layout">
      <div class="sidebar-header">
        <i class="bi bi-mortarboard-fill text-white fs-2 me-2"></i>
        <h3 class="m-0 fw-bold text-white">EduShare</h3>
      </div>
      
      <nav class="sidebar-nav">
        <router-link to="/dashboard" class="nav-item" :class="{ 'active': route.name === 'Home' }">
          <i class="bi bi-house-door"></i> Početna
        </router-link>
        
        <router-link v-if="isAdmin" to="/dashboard/admin" class="nav-item" :class="{ 'active': route.name === 'AdminPanel' }">
          <i class="bi bi-tools"></i> Upravljanje
        </router-link>

        <router-link to="/dashboard/subject" class="nav-item" :class="{ 'active': ['Subject', 'SubjectDetail'].includes(String(route.name)) }">
          <i class="bi bi-book"></i> Predmeti
        </router-link>

        <router-link to="/dashboard/codeshare" class="nav-item" :class="{ 'active': String(route.name) === 'CodeShare' }">
          <i class="bi bi-hash"></i> CodeShare
        </router-link>

        <router-link to="/dashboard/notification" class="nav-item" :class="{ 'active': String(route.name) === 'Notification' }">
          <i class="bi bi-bell"></i> Obaveštenja
          <span v-if="unreadCount > 0" class="notif-dot"></span>
        </router-link>
      </nav>
    </aside>

    <div class="main-container">
      <header class="navbar-top shadow-sm">
        <div class="user-meta">
          
          <div class="avatar-ring">{{ initials }}</div>
          <div class="user-details">
            <router-link to="/dashboard/profile" class="profile-link">Moj Profil</router-link>
            <span class="user-name">{{ displayName }}</span>
          </div>
          <button @click="handleLogout" class="btn-logout ms-4">Odjavi se</button>
        </div>
      </header>
      
      <main class="page-content">
        <router-view />
      </main>
    </div>
    
    <Session :show="showSession" @refresh="handleRefresh" @cancel="handleLogout" />
  </div>

  <div v-else class="full-page-loader">
    <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;"></div>
    <p class="mt-3 fw-bold text-muted">Učitavanje podataka...</p>
  </div>
</template>

<style scoped>
.dashboard-wrapper { display: flex; width: 100vw; height: 100vh; margin: 0; padding: 0; overflow: hidden; position: absolute; top: 0; left: 0; }
#sidebar-layout { width: 260px; min-width: 260px; background-color: #0d6efd; height: 100vh; flex-shrink: 0; display: flex; flex-direction: column; }
.main-container { flex-grow: 1; display: flex; flex-direction: column; min-width: 0; background-color: #f8f9fa; }
.navbar-top { height: 90px; background: white; display: flex; justify-content: flex-end; align-items: center; padding: 0 40px; border-bottom: 1px solid #dee2e6; width: 100%; flex-shrink: 0; z-index: 100; }
.sidebar-header { height: 90px; display: flex; align-items: center; padding: 0 25px; border-bottom: 1px solid rgba(255,255,255,0.1); }
.sidebar-nav { padding-top: 20px; }
.nav-item { display: flex; align-items: center; padding: 16px 25px; color: rgba(255,255,255,0.7); text-decoration: none; gap: 15px; transition: all 0.2s ease; font-weight: 500; border-left: 5px solid transparent; }
.nav-item:hover { background: rgba(255,255,255,0.1); color: white; }
.active { background: rgba(255,255,255,0.15); color: white !important; border-left: 5px solid white; }
.page-content { flex-grow: 1; overflow-y: auto; padding: 40px; }
.user-meta { display: flex; align-items: center; gap: 15px; }
.avatar-ring { width: 48px; height: 48px; border: 2px solid #0d6efd; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 800; color: #0d6efd; background: #f0f7ff; }
.user-details { display: flex; flex-direction: column; line-height: 1.2; }
.profile-link { font-size: 0.75rem; font-weight: 800; text-decoration: none; color: #0d6efd; text-transform: uppercase; }
.user-name { font-size: 0.95rem; font-weight: 700; color: #1e293b; }
.btn-logout { background: #fee2e2; color: #991b1b; border: none; padding: 10px 20px; border-radius: 10px; font-weight: 700; font-size: 0.85rem; transition: 0.2s; }
.btn-logout:hover { background: #fecaca; }
.notif-dot { width: 8px; height: 8px; background: #ff4d4d; border-radius: 50%; margin-left: auto; box-shadow: 0 0 5px #ff4d4d; }
.full-page-loader { height: 100vh; display: flex; flex-direction: column; justify-content: center; align-items: center; background: white; }
</style>