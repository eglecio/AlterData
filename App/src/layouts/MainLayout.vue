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
          <!-- <q-btn round dense flat color="white" icon="notifications" @click="marcarNovasNotificacoesComoVistas">
            <q-badge v-if="novasNotificacoes > 0" color="red" text-color="white" floating>
              {{ novasNotificacoes }}
            </q-badge>
            <q-menu
            >
              <q-list style="min-width: 100px">
                <messages></messages>
                <q-card class="text-center no-shadow no-border">
                  <q-btn label="Ver Todos" @click="$router.push('/Mensagens')" style="max-width: 120px !important;" flat dense
                         class="text-indigo-8"></q-btn>
                </q-card>
              </q-list>
            </q-menu>
          </q-btn> -->

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

        <q-item to="/usuarios" active-class="q-item-no-link-highlighting">
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
import Messages from "./Messages.vue";

import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'


export default defineComponent({
  name: 'MainLayout',

  components: {
    Messages
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
    // this.consultarNovasNotificacoes()
    // this.carregarLogo()
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

    async consultarNovasNotificacoes () {
      this.novasNotificacoes = 0
      try {
        const response = await api.post(`mensagem/novas`)
        if (response.data && response.data.Result > 0) {
          this.novasNotificacoes = response.data.Result
        }
      } catch (error) {
        console.log(error)
      }
    },

    async marcarNovasNotificacoesComoVistas (e, go) {
      try {
        if (this.novasNotificacoes === 0) {
          e.preventDefault()
          this.$router.push('/Mensagens')
          return false
        }
        const response = await api.post(`mensagem/visto`)
        this.novasNotificacoes = 0
      } catch (error) {
        console.log(error)
      }
    },

    async carregarLogo () {
      this.urlLogo = ''
      try {
        const response = await api.post(`empresa/logo`)
        this.urlLogo = response.data.Result
        if (this.urlLogo === undefined || this.urlLogo === null || this.urlLogo === 'NULL' || this.urlLogo.indexOf('user-avatar') > -1 || this.urlLogo === 'https://s3.us.cloud-object-storage.appdomain.cloud/npanexos/') {
          this.urlLogo = 'img/logo.png'
        } else {
          this.urlLogo = 'https://lbcloud.meuapp.fit/cdn-cgi/image/width=80/' + this.urlLogo
        }
      } catch (error) {
        console.log(error)
      }
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
