import { defineStore } from 'pinia';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: null as string | null,
  }),
  actions: {
    setToken(t: string) {
      this.token = t
      localStorage.setItem('token', t)
    },
  },
})