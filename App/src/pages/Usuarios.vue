<template>
  <q-page class="q-pa-sm">
    <div class="row" v-if="Object.keys(usuario_selecionado).length==0">
      <!-- CAMPO DE PESQUISA -->
      <div class="col-12">
        <q-card class="no-border no-shadow bg-transparent">
          <q-card-section class="q-pa-xs">
            <q-input rounded v-model="termoPesquisa" @update:model-value="pesquisar" outlined placeholder="Pesquisar por email do usuário">
              <template v-slot:append>
                <q-icon v-if="termoPesquisa === ''" name="search"/>
                <q-icon v-else name="clear" class="cursor-pointer" @click="termoPesquisa = '';resetar();pesquisar('')"/>
              </template>
            </q-input>
          </q-card-section>
        </q-card>
      </div>

      <!-- informativo quando nao ha registros -->
      <div v-if="usuarios.length === 0" class="col-12 text-center q-pa-md">
        <q-card class="no-border no-shadow bg-warning text-white">
          <q-card-section>
            <q-icon name="info" size="lg" />
            <span>Nenhum usuário para exibir.</span>
          </q-card-section>
        </q-card>
      </div>

      <!-- listagem de usuários com rolagem infinita -->
      <div class="col-12">
        <q-card class="fit no-shadow" bordered>
          <q-tab-panels v-model="tab" animated>
            <q-tab-panel name="contact" style="padding: 0">

              <q-list ref="scrollTargetRef" :style="'max-height: ' + alturaScroll + 'px; overflow: auto;'" class="rounded-borders" separator>
                <q-infinite-scroll ref="infinteScrollRef" @load="obterDados" :offset="alturaScroll" :scroll-target="scrollTargetRef">
                    <q-item
                      v-for="(usuario, index) in usuarios"
                      :key="index"
                      clickable
                      v-ripple
                      :class="(index % 2 === 0 ? 'bg-grey-3' : 'bg-white') + ' text-black'"
                      style="margin-bottom: 2px; border-bottom: 2px solid #D4D4D4; padding: 10px !important; padding-top: 15px !important; padding-bottom: 12px ;"
                    >

                      <q-item-section @click="carregar(usuario)">
                        <q-item-label lines="1" class="q-pb-xs">
                          <i class="fa-solid fa-circle-user text-black" style="font-size: 16px;"></i>
                          {{ usuario.email }}
                        </q-item-label>
                        <q-item-label caption lines="2">
                          <span class="text-black">
                            <i class="fa-regular fa-id-card text-black" style="font-size: 15px;"></i>
                            {{ usuario.perfil }}
                          </span>
                        </q-item-label>
                      </q-item-section>

                      <q-item-section @click="carregar(usuario)">
                        <q-item-label lines="1" class="q-pb-xs">
                          <i :class="`fas fa-circle ${ usuario.status === 1 ? 'text-blue' : 'text-red' }`" style="font-size: 15px;"></i>
                          {{ usuario.status === 1 ? "Ativo" : "Inativo" }}
                        </q-item-label>
                      </q-item-section>

                      <q-item-section side>
                        <div class="text-black q-gutter-xs">
                          <q-btn size="sm" flat dense round icon="edit" class="q-mr-xs" @click="editar(usuario)"/>
                          <q-btn size="sm" flat dense round icon="delete" class="q-mr-xs" @click="remover(usuario)"/>
                        </div>
                      </q-item-section>
                    </q-item>

                  <template v-slot:loading>
                    <div class="row justify-center q-my-md">
                      <q-spinner-dots color="primary" size="40px" />
                    </div>
                  </template>
                </q-infinite-scroll>
              </q-list>
            </q-tab-panel>
          </q-tab-panels>
        </q-card>
      </div>
    </div>

    <transition v-else
    class="col-12"
        appear
        enter-active-class="animated bounceInRight">
          <q-card class="no-border no-border" :style="{'height':size['height']-100+'px !important'}">
            <q-toolbar class="text-black ">
              <q-btn round flat class="q-pa-sm">
                <q-avatar size="80px">
                  <img src="img/user-avatar.png">
                </q-avatar>
              </q-btn>

              <q-item class="q-subtitle-1 q-pl-md">
                <q-item-section>
                  <q-item-label lines="1">{{ usuario_selecionado.email }}</q-item-label>
                  <q-item-label caption lines="1">
                    <span class="text-weight-bold">{{ usuario_selecionado.dataCadastro }}</span>
                  </q-item-label>
                  <q-item-label caption lines="1">
                    <q-badge v-if="usuario_selecionado.status === 1" color="blue" text-color="white" label="Ativo" />
                    <q-badge v-else color="red" text-color="white" label="Inativo" />
                  </q-item-label>
                </q-item-section>
              </q-item>

              <q-space/>

              <q-btn round flat icon="edit" @click="editar(usuario_selecionado)"/>
              <q-btn round flat icon="keyboard_backspace" @click="resetar();usuario_selecionado={}"/>
            </q-toolbar>
            <q-separator></q-separator>

            <div v-for="atributo, atributo_index in atributos_usuario">
              <contact-detail-item :icon="atributo.icon" :text_color="atributo.text_color"
                                  :value="usuario_selecionado[atributo['field']]" :label="atributo.label"></contact-detail-item>

              <q-separator inset="item" v-if="atributo_index!=atributos_usuario.length-1"></q-separator>
            </div>

          </q-card>
    </transition>

    <!-- confirmacao de remocao de usuario -->
    <q-dialog v-model="confirmacaoRemoverUsuario" persistent>
      <q-card>
        <q-card-section class="row items-center">
          <q-avatar icon="fas fa-user-slash" color="primary" text-color="white" />
          <span class="q-ml-sm text-justify q-pt-md">Deseja realmente remover o usuário "{{ usuarioEmail_ParaRemover }}"?</span>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="primary" v-close-popup @click="confirmacaoRemoverUsuario = false" />
          <q-btn label="Sim, quero remover" dense color="teal" v-close-popup @click="_executarRemover" />
        </q-card-actions>
      </q-card>
    </q-dialog>

    <q-footer elevated>
      <q-toolbar class="row">
        <q-btn
          fab
          color="teal-7"
          icon="fas fa-plus" padding="sm"
          class="absolute"
          style="top: 0; right: 12px; transform: translateY(-50%);"
          to="/usuario/novo"
        />
      </q-toolbar>
    </q-footer>
  </q-page>
</template>

<script>
import {defineComponent, defineAsyncComponent} from 'vue';
import {ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

const atributos_usuario = [
  {
    icon: 'mail',
    label: 'Email',
    field: 'email',
    text_color: 'grey-8'
  },
   {
    icon: 'fas fa-id-card',
    label: 'Perfil',
    field: 'perfilDescricao',
    text_color: 'grey-8'
  },
  {
    icon: 'far fa-calendar-check',
    label: 'Data de Cadastro',
    field: 'dataCadastro',
    text_color: 'grey-8'
  },
  {
    icon: 'fa-regular fa-calendar-xmark',
    label: 'Data de Inativação',
    field: 'dataInativacao',
    text_color: 'grey-8'
  },
  {
    icon: 'fas fa-user',
    label: 'Status',
    field: 'statusDescricao',
    text_color: 'grey-8'
  }
];

export default defineComponent({
  name: 'Usuarios',
  components: {
    ContactDetailItem: defineAsyncComponent(() => import('components/ContactDetailItem.vue'))
  },

  setup() {
    const termoPesquisa = ref('');
    const ultimaPesquisa = ref('');
    const scrollTargetRef = ref(null)
    const alturaScroll = document.documentElement.clientHeight - 200
    const confirmacaoRemoverUsuario = ref(false)
    const usuarioId_ParaRemover = ref(0)
    const usuarioEmail_ParaRemover = ref('')

    const $q = useQuasar()
    const size = ref({ width: '200px', height: '200px' });

    return {
      scrollTargetRef,
      usuarios: ref([]),
      tab: ref('contact'),
      termoPesquisa,
      ultimaPesquisa,
      pagina: 0,
      alturaScroll,
      usuario_selecionado: ref({}),
      size,
      atributos_usuario,
      confirmacaoRemoverUsuario,
      usuarioId_ParaRemover,
      usuarioEmail_ParaRemover
    }
  },

  methods: {

    async carregar (modelo) {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.get(`usuario/${modelo.id}`)
        if (response.data) {
          instance.usuario_selecionado = response.data
        }
      } catch (error) {
        console.log(error)
      }
      finally {
        Loading.hide()
      }
    },

    async obterDados (index, done) {
      var instance = this
      this.usuarios = this.pagina === 0 ? [] : this.usuarios
      this.pagina = index >= 0 ? index : (this.pagina + 1)

      try {
        var finalizarCarregamento = (possuiRegistros) => {
          setTimeout(() => {
            if (possuiRegistros === false) {
              instance.$refs.infinteScrollRef?.stop()
            }
            if (typeof done === "function") {
              done() // usado pra desaparecer o spinner...
            }
          }, index < 5 ? 1000 : 400)
        }

        const totalPorPagina = 12
        const response = await api.get(`usuario/${this.pagina}/${totalPorPagina}/${this.termoPesquisa}`)
        if (response.data && response.data.length > 0) {
          this.usuarios.push(...response.data)
        }

        if (index >= 0) {
          finalizarCarregamento(response.data.length > 0)
        }

      } catch (error) {
        console.log(error)
        instance.$refs.infinteScrollRef.stop()
      }

    },

    pesquisar (e) {
      var instance = this
      this.pagina = 0

      setTimeout(async () => {
        if (e === instance.termoPesquisa && instance.termoPesquisa !== '') {
          instance.ultimaPesquisa = instance.termoPesquisa
          // instance.$refs.infinteScrollRef.resume()
        } else if (e === '' && instance.ultimaPesquisa !== '') {
          instance.termoPesquisa = ''
          instance.ultimaPesquisa = ''
          // instance.$refs.infinteScrollRef.resume()
        }
        instance.usuarios = []
        instance.$refs.infinteScrollRef.reset()
        setTimeout(() => {
          instance.$refs.infinteScrollRef.resume()
        }, 500)
      }, e === '' ? 50 : 1000)
    },

    resetar () {
      this.pagina = 0
      this.usuarios = []
    },

    editar (modelo) {
      this.$router.push(`/usuario/${modelo.id}`)
    },

    remover (modelo) {
      this.usuarioId_ParaRemover = modelo.id
      this.usuarioEmail_ParaRemover = modelo.email
      this.confirmacaoRemoverUsuario = true
    },

    async _executarRemover () {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.delete(`usuario/${instance.usuarioId_ParaRemover}`)
        if (response.status === 200) {
          Notify.create({
            message: `Usuário removido com sucesso`,
            color: 'positive',
            icon: 'check',
            position: 'center',
            timeout: 2500
          })
          instance.usuarios = []
          instance.obterDados(1)
        } else {
          Notify.create({
            message: 'Não foi possível remover o usuário, tente novamente...',
            color: 'negative',
            icon: 'warning',
            position: 'center',
            timeout: 2500
          })
        }
      } catch (error) {
        console.log(error)
      }
      finally {
        Loading.hide()
      }
    }
  }
})
</script>
