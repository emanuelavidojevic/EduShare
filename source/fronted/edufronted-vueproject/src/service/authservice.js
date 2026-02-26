import api from "./api";

const AUTH_URL = 'Auth/';

const startExpiryTimer = (expiryTime) => {
    const expiresAt = new Date(expiryTime).getTime();
    const now = new Date().getTime();
    const timeLeft = expiresAt - now;

    if (timeLeft > 0) {
        setTimeout(async () => {
            if (window.confirm("Vaša sesija od 1 min je upravo istekla. Želite li da je produžite?")) {
                try {
                    const res = await api.post(AUTH_URL + 'refresh-access-token');
                    startExpiryTimer(res.data.expiry);
                } catch (err) {
                    window.location.href = '/login';
                }
            } else {
                window.location.href = '/login';
            }
        }, timeLeft);
    }
};

export const login = async (Email, Password) => {
    const response = await api.post(AUTH_URL + 'login', { Email, Password });
    if (response.data && response.data.expiry) {
        startExpiryTimer(response.data.expiry);
    }
    return response.data;
};

export const register = async (userData) => {
    const response = await api.post(AUTH_URL + 'register', userData);
    return response.data;
};

export const logout = async () => {
    await api.post(AUTH_URL + 'logout');
    window.location.href = '/login';
};