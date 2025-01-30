<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-btn
          flat
          dense
          round
          @click="toggleLeftDrawer"
          icon="menu"
          aria-label="Menu"
        />
        <q-toolbar-title>
          <!-- Admin Alterdata -->
          {{ [ undefined, null, '' ].indexOf(this.$route.meta.titulo) > -1 ? 'Admin Alterdata' : this.$route.meta.titulo }}
        </q-toolbar-title>
        <!-- <q-space/> -->
        <div class="q-gutter-sm row items-center no-wrap">
          <q-btn round dense flat color="white" :icon="$q.fullscreen.isActive ? 'fullscreen_exit' : 'fullscreen'"
              @click="$q.fullscreen.toggle()"
              v-if="$q.screen.gt.sm">
          </q-btn>
          <!-- icone da empresa -->
          <q-btn round flat>
            <q-avatar size="40px">
              <img :src="urlLogo" loading="lazy" style="object-fit: fill;">
            </q-avatar>
          </q-btn>
        </div>
      </q-toolbar>
    </q-header>

    <q-drawer
      v-model="leftDrawerOpen"
      show-if-above
      bordered
      class="bg-primary text-white"
    >
      <q-list>
        <q-item to="/" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="dashboard"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Dashboard</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/clientes" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="fas fa-users"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Clientes</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/produtos" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="fas fa-light fa-cubes"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Produtos</q-item-label>
          </q-item-section>
        </q-item>

        <q-item v-if="perfilPermissao === 99" to="/usuarios" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="fas fa-regular fa-users-gear"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Usuários</q-item-label>
          </q-item-section>
        </q-item>

        <q-item to="/" @click="logoff" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="exit_to_app"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Sair</q-item-label>
          </q-item-section>
        </q-item>

        <!-- <q-item to="/Charts" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="insert_chart"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Charts</q-item-label>
          </q-item-section>
        </q-item> -->

        <!-- <q-item to="/Tables" active-class="q-item-no-link-highlighting">
          <q-item-section avatar>
            <q-icon name="table_chart"/>
          </q-item-section>
          <q-item-section>
            <q-item-label>Tables</q-item-label>
          </q-item-section>
        </q-item> -->

      </q-list>
    </q-drawer>

    <q-page-container class="bg-grey-2">
      <router-view/>
    </q-page-container>
  </q-layout>
</template>

<script>
import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

const perfilPermissaoEnum = {
  Padrao: 1,
  Editor: 2,
  Admin: 99
}

export default defineComponent({
  name: 'MainLayout',

  components: {
  },

  setup() {
    const leftDrawerOpen = ref(false)
    const $q = useQuasar()
    const novasNotificacoes = ref(0)
    const urlLogo = ref('img/user-avatar.png')

    return {
      $q,
      leftDrawerOpen,
      toggleLeftDrawer() {
        leftDrawerOpen.value = !leftDrawerOpen.value
      },
      novasNotificacoes,
      urlLogo
    }
  },

  created () {
    this.verificarToken()
  },

  mounted () {
    this.obterPerfilUsuario()
  },

  methods: {

    logoff (e, go) {
      e.preventDefault()
      LocalStorage.clear()
      this.$router.push('/login')
    },

    verificarToken () {
      var token = LocalStorage.getItem('token')
      var valoresInvalidos = [ null, undefined, '']
      if (valoresInvalidos.indexOf(token) > -1) {
        this.$router.push('/login')
      } else {
        // Configura para todas as demais chamadas o token padrao no cabecalho...
        api.defaults.headers = { 'x-access-token': token, 'Content-Type': 'application/json;charset=UTF-8' }
      }
      // TODO: precisa ter uma metodo que solicita ao ws se o token do cara ainda eh valido, se ele esta adimplente, etc...
    },

    async obterPerfilUsuario () {
      await api.get('usuario/perfil')
        .then((response) => {
          switch (response.data) {
            case 'Admin':
              this.perfilPermissao = perfilPermissaoEnum.Admin
              break
            case 'Editor':
              this.perfilPermissao = perfilPermissaoEnum.Editor
              break
            default:
              this.perfilPermissao = perfilPermissaoEnum.Padrao
              break
          }
          LocalStorage.set('perfilPermissao', this.perfilPermissao)
        })
        .catch((error) => {
          console.error(error)
        })
    }



  }
})
</script>

<style>

/* FONT AWESOME GENERIC BEAT */
.fa-beat {
  animation: fa-beat 5s ease infinite;
}

@keyframes fa-beat {
  0% {
    transform: scale(1);
  }
  5% {
    transform: scale(1.25);
  }
  20% {
    transform: scale(1);
  }
  30% {
    transform: scale(1);
  }
  35% {
    transform: scale(1.25);
  }
  50% {
    transform: scale(1);
  }
  55% {
    transform: scale(1.25);
  }
  70% {
    transform: scale(1);
  }
}

</style>
