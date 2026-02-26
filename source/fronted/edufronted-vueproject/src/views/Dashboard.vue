<script setup>
import { useRouter } from 'vue-router';
const router = useRouter();

const handleLogout = () => {
  router.push('/login');
};
</script>

<template>
  <div class="d-flex w-100" id="main-layout">
    <aside id="sidebar">
      <div class="sidebar-header text-center">
        <h4 class="m-0 fw-bold text-white">EduShare</h4>
      </div>
      
      <nav class="sidebar-nav">
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
      <header class="navbar-custom d-flex justify-content-end align-items-center px-4 gap-3">
        <router-link to="/dashboard/profile" class="nav-btn">My Profile</router-link>
        <button @click="handleLogout" class="nav-btn logout-hover">Log out</button>
      </header>

      <section class="p-4 content-area">
        <router-view />
      </section>
    </main>
  </div>
</template>

<style scoped>
#main-layout { min-height: 100vh; }
#sidebar {
  width: 220px;
  background-color: #0d6efd;
  min-height: 100vh;
  flex-shrink: 0;
}
.sidebar-header { padding: 2rem 1.5rem; border-bottom: 1px solid rgba(255, 255, 255, 0.1); }
.nav-link {
  color: rgba(255, 255, 255, 0.8);
  padding: 1rem 1.5rem;
  text-decoration: none;
  display: block;
  transition: 0.2s;
  border-left: 4px solid transparent;
}
.nav-link:hover { color: white; background-color: rgba(255, 255, 255, 0.1); }

/* --- REŠENJE ZA SVETLJENJE --- */

/* 1. Za sve linkove (osim Početne), neka svetle kad god putanja počinje tim imenom */
.nav-link.router-link-active:not(.home-link) {
  background-color: rgba(255, 255, 255, 0.2);
  color: white;
  border-left: 4px solid white;
}

/* 2. Za Početnu (Home) koristimo EXACT meč, da ne bi svetlela dok si u Predmetima */
.home-link.router-link-exact-active {
  background-color: rgba(255, 255, 255, 0.2);
  color: white;
  border-left: 4px solid white;
}

/* Stilovi za header i ostalo */
.navbar-custom { height: 65px; background-color: white; border-bottom: 1px solid #e9ecef; }
.nav-btn { border: none; background: transparent; color: #6c757d; font-weight: 600; text-decoration: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; }
.logout-hover:hover { background-color: #dc3545 !important; color: white !important; }
</style>