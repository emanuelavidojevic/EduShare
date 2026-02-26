import api from "./api";

const AUTH_URL = 'Auth/';

export const login = async (Email, Password) => {
    const response = await api.post(AUTH_URL + 'login', { Email, Password });
    return response.data;
};

export const register = async (userData) => {
    const response = await api.post(AUTH_URL + 'register', userData);
    return response.data;
};

export const logout = async () => {
    try {
        await api.post(AUTH_URL + 'logout');
    } catch (e) {
        console.error("Logout error", e);
    } finally {
        localStorage.removeItem('user_display'); 
        window.location.href = '/login';
    }
};

export const refreshSession = async () => {
    const response = await api.post(AUTH_URL + 'refresh-access-token');
    return response.data;
};