import { createRouter, createWebHistory } from 'vue-router'
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
      children: [
        { 
          path: '', // Prazno znači da je ovo baza: /dashboard
          name: 'Home',
          component: Home 
        },
        { 
          path: 'subject', // Relativno: /dashboard/subject
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
        }
      ]
    }
  ]
})

export default router