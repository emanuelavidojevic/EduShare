import axios from "axios";

const api = axios.create({
    baseURL: 'http://localhost:5145/api/',
    withCredentials: true // Ovo omogućava slanje i primanje cookie-ja
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;
        
        // Ako je 401, znači da je Access Cookie istekao
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            try {
                // ISPRAVLJENO: Putanja mora biti ista kao na backendu
                await axios.post('http://localhost:5145/api/Auth/refresh-access-token', {}, { withCredentials: true });
                
                // Ako refresh uspe, ponovi originalni zahtev
                return api(originalRequest);
            } catch (refreshError) {
                // Ako i refresh pukne, sesija je stvarno mrtva
                return Promise.reject(refreshError);
            }
        }
        return Promise.reject(error);
    }
);

export default api;