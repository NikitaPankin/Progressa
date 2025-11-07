import axios from "axios";
import type { InternalAxiosRequestConfig } from "axios";
import jwtDecode from "jwt-decode";
import type { TokenPair } from "../utils/tokenStorage.ts";
import {
  getAccessToken,
  getRefreshToken,
  saveTokens,
  removeTokens,
} from "../utils/tokenStorage.ts";

interface JwtPayload {
  exp: number;
}

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "https://localhost:32769/api";
export const api = axios.create({
  baseURL: API_BASE_URL,
  withCredentials: true,
})

api.interceptors.request.use(async (config: InternalAxiosRequestConfig) => {
  if (config.url?.includes('/session/signin') || config.url?.includes('/session/register')) {
    return config
  }

  let accessToken = getAccessToken();
  const refreshToken = getRefreshToken();

  if (accessToken) {
    try {
      const decoded = jwtDecode<JwtPayload>(accessToken);

      if (Date.now() + 5000 > decoded.exp * 1000) {
        if (!refreshToken) throw new Error("Refresh token missing");

        const response = await axios.post<TokenPair>(
          `${API_BASE_URL}/session/refresh`,
          refreshToken,
          { headers: { "Content-Type": "application/json" } }
        );

        const newAccess = response.data.accessToken;
        const newRefresh = response.data.refreshToken;

        if (!newAccess || !newRefresh) {
          removeTokens();
          return config;
        }

        saveTokens({ accessToken: newAccess, refreshToken: newRefresh });
        accessToken = newAccess;
      }

      config.headers = config.headers ?? {};
      config.headers["Authorization"] = `Bearer ${accessToken}`;
    } catch (err) {
      console.error("Token error:", err);
    }
  }

  return config;
});