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
      <div class="text-center q-pa-md q-mb-md" style="border-bottom: 1px solid #5064b5;">
        <span class="text-weight-light text-caption">{{ login }}</span>
      </div>
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

        <q-item v-if="perfilPermissao === perfilPermissaoEnum.Admin" to="/usuarios" active-class="q-item-no-link-highlighting">
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

export default defineComponent({
  name: 'MainLayout',

  components: {
  },

  setup() {
    const perfilPermissaoEnum = {
      Padrao: 1,
      Editor: 2,
      Admin: 99
    }
    const leftDrawerOpen = ref(false)
    const $q = useQuasar()
    const urlLogo = ref('img/user-avatar.png')
    return {
      $q,
      leftDrawerOpen,
      toggleLeftDrawer() {
        leftDrawerOpen.value = !leftDrawerOpen.value
      },
      urlLogo,
      login: ref(''),
      perfilPermissaoEnum
    }
  },

  created () {
  },

  mounted () {
    this.obterPerfilUsuario()
    this.login = LocalStorage.getItem('login')
  },

  methods: {

    efetuarLogoutSessaoExpirada () {
      Notify.create({
        type: 'negative',
        message: 'Sessão expirada. Por favor, faça login novamente.',
        position: 'center',
        timeout: 3000
      })
      this.logoff()
      return
    },

    logoff (e, go) {
      e?.preventDefault()
      localStorage.clear()
      this.$router.push('/login')
    },

    async obterPerfilUsuario () {
      try {
        var response = await api.get('usuario/perfil')
        switch (response.data) {
          case 'Admin':
            this.perfilPermissao = this.perfilPermissaoEnum.Admin
            break
          case 'Editor':
            this.perfilPermissao = this.perfilPermissaoEnum.Editor
            break
          default:
            this.perfilPermissao = this.perfilPermissaoEnum.Padrao
            break
        }

        LocalStorage.set('perfilPermissao', this.perfilPermissao)
      } catch (error) {
        if (error.response?.status === 401 || error.stack.indexOf('Network Error') > -1) {
          this.efetuarLogoutSessaoExpirada()
        }
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
