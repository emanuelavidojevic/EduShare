<script setup>
import { ref, onMounted, onUnmounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import Session from './Session.vue'; 

const router = useRouter();
const userName = ref('');
const userLastName = ref('');
const showSession = ref(false);
let expiryTimer = null;

const startTimer = (expiryTime) => {
  if (expiryTimer) clearTimeout(expiryTimer);
  const timeLeft = new Date(expiryTime).getTime() - new Date().getTime();
  if (timeLeft > 0) {
    expiryTimer = setTimeout(() => { showSession.value = true; }, timeLeft);
  }
};

const handleRefresh = async () => {
  try {
    const res = await axios.post('http://localhost:5145/api/Auth/refresh-access-token', {}, { withCredentials: true });
    showSession.value = false;
    if (res.data?.expiry) {
      startTimer(res.data.expiry);
      const data = JSON.parse(localStorage.getItem('user_display') || '{}');
      data.expiry = res.data.expiry;
      localStorage.setItem('user_display', JSON.stringify(data));
    }
  } catch (err) { handleLogout(); }
};

const loadUserData = async () => {
  const localData = localStorage.getItem('user_display');
  if (localData) {
    const user = JSON.parse(localData);
    userName.value = user.firstName || '';
    userLastName.value = user.lastName || '';
    if (user.expiry) startTimer(user.expiry);
  }

  try {
    const res = await axios.get('http://localhost:5145/api/Auth/me', { withCredentials: true });
    if (res.data) {
      userName.value = res.data.firstName;
      userLastName.value = res.data.lastName;
      localStorage.setItem('user_display', JSON.stringify({ ...res.data }));
    }
  } catch (e) { console.error(e); }
};

const handleLogout = async () => {
  try { await axios.post('http://localhost:5145/api/Auth/logout', {}, { withCredentials: true }); } 
  finally {
    localStorage.removeItem('user_display');
    showSession.value = false;
    if (expiryTimer) clearTimeout(expiryTimer);
    router.push('/login');
  }
};

const initials = computed(() => {
  return ((userName.value?.[0] || '') + (userLastName.value?.[0] || '')).toUpperCase() || '??';
});

onMounted(() => {
  loadUserData();
  axios.interceptors.response.use(r => r, e => {
    if (e.response?.status === 401) showSession.value = true;
    return Promise.reject(e);
  });
});

onUnmounted(() => { if (expiryTimer) clearTimeout(expiryTimer); });
</script>

<template>
  <div class="d-flex w-100" id="main-layout">
    <aside id="sidebar">
      <div class="sidebar-header d-flex align-items-center px-3">
        <i class="bi bi-mortarboard-fill text-white fs-2 me-2"></i>
        <h3 class="m-0 fw-bold text-white sidebar-logo-text">EduShare</h3>
      </div>
      
      <nav class="sidebar-nav mt-2">
        <router-link to="/dashboard" class="nav-link home-link">
          <i class="bi bi-house-door me-2"></i> Početna
        </router-link>
        <router-link to="/dashboard/subject" class="nav-link">
          <i class="bi bi-book me-2"></i> Predmeti
        </router-link>
        <router-link to="/dashboard/codeshare" class="nav-link">
          <i class="bi bi-hash me-2"></i> CodeShare
        </router-link>
        <router-link to="/dashboard/notification" class="nav-link">
          <i class="bi bi-bell me-2"></i> Obaveštenja
        </router-link>
      </nav>
    </aside>

    <main class="flex-grow-1 bg-light">
      <header class="navbar-custom d-flex justify-content-end align-items-center px-4 gap-4 shadow-sm">
        <div class="avatar-circle">
          {{ initials }}
        </div>
        <div class="profile-group text-center">
          <router-link to="/dashboard/profile" class="nav-btn-profile">My Profile</router-link>
          <div class="user-full-name">{{ userName }} {{ userLastName }}</div>
        </div>
        <button @click="handleLogout" class="nav-btn logout-hover">Log out</button>
      </header>

      <section class="p-4 content-area">
        <router-view />
      </section>
    </main>

    <Session 
      :show="showSession" 
      @refresh="handleRefresh" 
      @cancel="handleLogout" 
    />
  </div>
</template>

<style scoped>
#main-layout { min-height: 100vh; }
#sidebar { width: 240px; background-color: #0d6efd; min-height: 100vh; flex-shrink: 0; box-shadow: 4px 0 10px rgba(0,0,0,0.05); }
.sidebar-header { height: 75px; border-bottom: 1px solid rgba(255, 255, 255, 0.1); }
.sidebar-logo-text { font-size: 1.6rem; letter-spacing: -0.5px; line-height: 1; }
.nav-link { color: rgba(255, 255, 255, 0.8); padding: 0.9rem 1.5rem; text-decoration: none; display: block; transition: 0.2s; border-left: 4px solid transparent; font-size: 1.05rem; font-weight: 500; }
.nav-link:hover { color: white; background-color: rgba(255, 255, 255, 0.1); }
.nav-link.router-link-active:not(.home-link) { background-color: rgba(255, 255, 255, 0.2); color: white; border-left: 4px solid white; }
.home-link.router-link-exact-active { background-color: rgba(255, 255, 255, 0.2); color: white; border-left: 4px solid white; }
.navbar-custom { height: 75px; background-color: white; border-bottom: 1px solid #e9ecef; }
.profile-group { display: flex; flex-direction: column; align-items: center; gap: 0; }
.nav-btn-profile { color: #0d6efd; font-weight: 700; text-decoration: none; font-size: 0.95rem; }
.user-full-name { font-size: 0.75rem; color: #6c757d; font-weight: 500; margin-top: -2px; }
.avatar-circle { width: 42px; height: 42px; background-color: #f8f9fa; color: #0d6efd; border: 2px solid #0d6efd; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: 800; font-size: 0.85rem; transition: transform 0.2s; }
.avatar-circle:hover { transform: scale(1.05); }
.nav-btn { border: none; background: transparent; color: #6c757d; font-weight: 600; padding: 8px 15px; border-radius: 6px; cursor: pointer; }
.logout-hover:hover { background-color: #dc3545 !important; color: white !important; transition: 0.3s; }
.content-area { animation: fadeIn 0.3s ease-in-out; }
@keyframes fadeIn { from { opacity: 0; } to { opacity: 1; } }
</style>