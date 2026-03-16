import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useThemeStore = defineStore('theme', () => {
  const isDark = ref(localStorage.getItem('theme') === 'dark');

  const toggleTheme = () => {
    isDark.value = !isDark.value;
    const theme = isDark.value ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
  };

  const initTheme = () => {
    const theme = isDark.value ? 'dark' : 'light';
    document.documentElement.setAttribute('data-theme', theme);
  };

  return { isDark, toggleTheme, initTheme };
});