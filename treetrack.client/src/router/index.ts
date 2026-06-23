import { createRouter, createWebHistory, type RouteRecordRaw, type NavigationGuardNext, type RouteLocationNormalized } from 'vue-router'
import { useAuthStore } from '@/stores/authStore'
import LoginPage from '@/pages/LoginPage.vue'
import RegisterPage from '@/pages/RegisterPage.vue'
import WorkspacePage from '@/pages/WorkspacePage.vue'
import NotFoundPage from '@/pages/NotFoundPage.vue'
import IssuesPage from '@/pages/IssuesPage.vue'
import InvitePage from '@/pages/InvitePage.vue'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    redirect: { name: 'Workspace' }
  },
  {
    path: '/workspace',
    name: 'Workspace',
    component: WorkspacePage,
    meta: { requiresAuth: true, layout: 'issues' }
  },
  {
    path: '/projects/:projectId/issues',
    name: 'ProjectIssues',
    component: IssuesPage,
    meta: { requiresAuth: true, layout: 'issues' }
  },
  {
    path: '/issues',
    redirect: { name: 'Workspace' }
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginPage,
    meta: { requiresAuth: false, layout: 'auth' }
  },
  {
    path: '/register',
    name: 'Register',
    component: RegisterPage,
    meta: { requiresAuth: false, layout: 'auth' }
  },
  {
    path: '/invite/:token',
    name: 'Invite',
    component: InvitePage,
    meta: { requiresAuth: false, layout: 'auth' }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: NotFoundPage,
    meta: { layout: 'issues' }
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes
})

router.beforeEach(
  (to: RouteLocationNormalized, _from: RouteLocationNormalized, next: NavigationGuardNext) => {
    const authStore = useAuthStore()

    if (!authStore.isInitialized) {
      const unwatch = authStore.$subscribe(() => {
        if (authStore.isInitialized) {
          unwatch()
          router.push(to.fullPath)
        }
      })
      return
    }

    const requiresAuth = to.meta.requiresAuth === true

    if (requiresAuth) {
      if (authStore.isAuthenticated) {
        next()
      } else {
        next({
          name: 'Login',
          query: { redirect: to.fullPath }
        })
      }
    } else if (
      authStore.isAuthenticated &&
      (to.name === 'Login' || to.name === 'Register')
    ) {
      next({ name: 'Workspace' })
    } else {
      next()
    }
  }
)

export default router
