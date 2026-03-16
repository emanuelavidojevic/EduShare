import api from "./api";
import { useAuthStore } from "../stores/auth.js";

const AUTH_URL = 'Auth/';

export const fetchMe = async () => {
    const authStore = useAuthStore();
    try {
        const response = await api.get(AUTH_URL + 'me');
        if (response.status === 200) {
            authStore.setUser(response.data);
            return response.data;
        }
    } catch (e) {
        authStore.logout();
        return null;
    }
};

export const login = async (Email, Password) => {
    const authStore = useAuthStore();
    const response = await api.post(AUTH_URL + 'login', { Email, Password });
    
    if (response.status === 200) {
        await fetchMe();
    }
    return response.data;
};

export const register = async (userData) => {
    const response = await api.post(AUTH_URL + 'register', userData);
    return response.data;
};

export const logout = async () => {
    const authStore = useAuthStore();
    try {
        await api.post(AUTH_URL + 'logout');
    } catch (e) {
        console.error("Logout error", e);
    } finally {
        authStore.logout();
        window.location.href = '/login';
    }
};

export const refreshSession = async () => {
    const authStore = useAuthStore();
    const response = await api.post(AUTH_URL + 'refresh-access-token');
    
    if (response.status === 200) {
        authStore.login(true);
    }
    return response.data;
};