import { createApp } from 'vue';
import App from './App.vue';
import './assets/tailwind.css';
import { router } from './router'
import { createPinia } from 'pinia';

const app = createApp(App)
app.mount('#app')
app.use(router)
const pinia = createPinia()
app.use(pinia)