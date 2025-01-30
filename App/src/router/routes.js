const routes = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      {path: '', component: () => import('pages/Dashboard.vue')},
      {path: '/Clientes', component: () => import('src/pages/Clientes.vue'), meta: { titulo: 'Clientes' }},
      {path: '/Cliente/:id?', component: () => import('src/pages/Cliente.vue')},
      {path: '/Produtos', component: () => import('src/pages/Produtos.vue'), meta: { titulo: 'Produtos' }},
      {path: '/Produto/:id?', component: () => import('src/pages/Produto.vue')},
      {path: '/Usuarios', component: () => import('src/pages/Usuarios.vue'), meta: { titulo: 'Usuários' }},
      {path: '/Usuario/:id?', component: () => import('src/pages/Usuario.vue')}
    ]
  },
  {
    path: '/Login',
    component: () => import('pages/Login.vue')
  },
  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/Error404.vue')
  }
]

export default routes
