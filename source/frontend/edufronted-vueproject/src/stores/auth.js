import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    isAuthenticated: false,
  }),
  actions: {
    login(userData) {
      this.isAuthenticated = true;
      if (typeof userData === 'object') {
        this.user = userData;
      }
    },
    logout() {
      this.isAuthenticated = false;
      this.user = null;
    },
    setUser(userData) {
      this.user = userData;
      this.isAuthenticated = true;
    }
  }
});