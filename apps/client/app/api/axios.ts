import axios from "axios";

const API = axios.create({
  baseURL: "",
  withCredentials: true,
});

// Interceptor
API.interceptors.response.use(
  (res) => res,
  (error) => {
    if (error.response?.status === 401) {
      alert("Session expired!, please login again.");
      window.location.href = "./signin";
      return Promise.reject(error);
    }
  },
);

export default API;
