import { createRouter, createWebHistory } from 'vue-router';
import HomePage from '../pages/HomePage.vue'
import DashboardPage from '../pages/DashboardPage.vue'
import ChallengesPage from '../pages/ChallengesPage.vue'
import RemindersPage from '../pages/RemindersPage.vue'
import SocialPage from '../pages/SocialPage.vue'

const routes = [
  { path: '/', component: HomePage },
  { path: '/dashboard', component: DashboardPage },
  { path: '/challenges', component: ChallengesPage },
  { path: '/reminders', component: RemindersPage },
  { path: '/social', component: SocialPage },
]

export const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router;