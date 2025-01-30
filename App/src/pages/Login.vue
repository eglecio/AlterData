<template>
  <q-layout>
    <q-page-container>
      <q-page class="flex bg-image flex-center">
        <q-card v-bind:style="$q.screen.lt.sm?{'width': '80%'}:{'width':'30%'}">
          <q-card-section style="margin-bottom: 80px;">
            <!-- <q-avatar size="103px" class="absolute-center shadow-10">
              <img src="profile.svg">
            </q-avatar> -->
            <div style="border-radius: 5px" class="absolute-center shadow-10">
              <img loading="lazy" style="width: 250px;border-radius: 5px" class="bg-white" src="img/logo.png">
            </div>
          </q-card-section>
          <q-card-section>
            <div class="text-center q-pt-xl">
              <div class="col text-h6 ellipsis">
                <!-- Log in -->
              </div>
            </div>
          </q-card-section>
          <q-card-section>
            <q-form
              class="q-gutter-md"
            >
              <q-input
                filled
                v-model="login"
                label="E-mail"
                lazy-rules
              />

              <q-input
                type="password"
                filled
                v-model="senha"
                label="Senha"
                lazy-rules

              />

              <div>
                <q-btn label="Login" to="/" @click="logar" type="button" color="green" class="full-width q-mt-lg"/>
              </div>
            </q-form>
          </q-card-section>
        </q-card>
      </q-page>
    </q-page-container>
  </q-layout>
</template>

<script>
import {defineComponent} from 'vue'
import {ref} from 'vue'
import { api } from 'boot/axios'
// import { handleAxiosError } from 'utils/erroHandler.js'
import { Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'
// import { SessionStorage } from 'quasar'

export default defineComponent({

  setup() {
    // const $q = useQuasar()

    // const setTokenPadraoParaRequisicoes = (tokenParametro = LocalStorage.getItem('token')) => {
    //   if (tokenParametro) {
    //     api.defaults.headers = { 'x-access-token': tokenParametro, 'Content-Type': 'application/json;charset=UTF-8' }
    //   }
    // }

    return {
      login: ref(''),
      senha: ref('')
      // setTokenPadraoParaRequisicoes
    }
  },

  mounted () {
    // this.setTokenPadraoParaRequisicoes()
  },

  methods: {

    async logar (e, go)  {
      e.preventDefault()
      Loading.show({ spinner: QSpinnerGears })

      await api.post(`usuario/login`, { Login: this.login, Senha: this.senha })
        .then((response) => {
          if (response.data) {
            LocalStorage.set('tokenCompleto', response.data)
            LocalStorage.set('token', response.data.accessToken)
            LocalStorage.set('tokenType', response.data.tokenType)
            LocalStorage.set('refreshToken', response.data.refreshToken)
            LocalStorage.set('login', this.login)
            // this.setTokenPadraoParaRequisicoes(response.data.accessToken)
            this.$router.push('/')
            return
          }
          LocalStorage.clear()
          // Em tese nao chegara nesse ponto, mas caso chegue o usuario tera uma informacao ao menos...
          Notify.create({
            color: 'warning',
            icon: 'fa fa-stop',
            message: 'Usuário e Senha inválidos',
            position: 'center',
            timeout: 2000
          })
          this.$router.push('/login')
        })
        .catch((error) => {
          console.error(error)
          // Caso webservice fora do ar, ou erro de rede...
          if (!error.response) {
            Notify.create({
              message: 'Ops! Ocorreu um erro ao tentar logar. Tente novamente mais tarde.',
              position: 'center',
              timeout: 3000
            })
          }
        })
        .finally(() => {
          Loading.hide()
          this.login = ''
          this.senha = ''
        })
    }
  }
})
</script>

<style>

.bg-image {
  /* background-image: linear-gradient(135deg, #7028e4 0%, #e5b2ca 100%); */
  background-image: linear-gradient(135deg, #bacfef 0%, #fff 100%);
}
</style>
