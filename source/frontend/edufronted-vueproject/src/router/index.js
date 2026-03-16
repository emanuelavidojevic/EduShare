import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import LogIn from '../views/LogIn.vue'
import Register from '../views/Register.vue'
import Dashboard from '@/views/Dashboard.vue'
import Subject from '@/views/Subject.vue'
import Home from '@/views/Home.vue'
import Profile from '@/views/Profile.vue'
import CodeShare from '@/views/CodeShare.vue'
import Notification from '@/views/Notification.vue'

const router = createRouter({
  history: createWebHistory('/'),
  routes: [
    {
      path: '/',
      redirect: '/login',
    },
    {
      path: '/login',
      name: 'LogIn',
      component: LogIn,
    },
    {
      path: '/register',
      name: 'Register',
      component: Register
    },
    {
      path: '/dashboard', 
      component: Dashboard,
      meta: { requiresAuth: true },
      children: [
        { 
          path: '', 
          name: 'Home',
          component: Home 
        },
        { 
          path: 'subject', 
          name: 'Subject',
          component: Subject
        },
        {
          path: 'subject/:id', 
          name: 'SubjectDetail',
          component: () => import('@/views/SubjectDetail.vue')
        },
        { 
          path: 'profile', 
          name: 'Profile',
          component: Profile
        },
        {
          path: 'codeshare',
          name: 'CodeShare',
          component: CodeShare
        },
        { 
          path: 'notification', 
          name: 'Notification',
          component: Notification
        },
        {
          path: 'admin',
          name: 'AdminPanel',
          component: () => import('@/views/AdminPanel.vue'), // Napravi ovaj view ako ne postoji
          meta: { requiresAdmin: true }
        }
      ]
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/dashboard'
    }
  ]
})

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const isAuthenticated = authStore.isAuthenticated;
  const userRole = authStore.user?.role || authStore.user?.Role;

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login');
  } 
  else if ((to.name === 'LogIn' || to.name === 'Register') && isAuthenticated) {
    next('/dashboard');
  }
  // Zastita za Admin panel - Provera da li ruta zahteva Admin rolu
  else if (to.meta.requiresAdmin && userRole !== 'Admin') {
    next('/dashboard'); // Ako nije admin, baci ga na pocetnu dashboard-a
  }
  else {
    next();
  }
});

export default router;