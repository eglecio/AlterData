<template>
  <div class="row" v-if="Object.keys(cliente_selecionado).length==0">
    <!-- CAMPO DE PESQUISA -->
    <div class="col-12">
      <q-card class="no-border no-shadow bg-transparent">
        <q-card-section class="q-pa-xs">
          <q-input rounded v-model="termoPesquisa" @update:model-value="pesquisar" outlined placeholder="Pesquisar por (nome/email/fone)">
            <template v-slot:append>
              <q-icon v-if="termoPesquisa === ''" name="search"/>
              <q-icon v-else name="clear" class="cursor-pointer" @click="termoPesquisa = '';resetar();pesquisar('')"/>
            </template>
          </q-input>
        </q-card-section>
      </q-card>
    </div>

    <!-- informativo quando nao ha clientes -->
    <div v-if="clientes.length === 0" class="col-12 text-center q-pa-md">
      <q-card class="no-border no-shadow bg-warning text-white">
        <q-card-section>
          <q-icon name="info" size="lg" />
          <span>Nenhum cliente para exibir.</span>
        </q-card-section>
      </q-card>
    </div>

    <!-- listagem de clientes com rolagem infinita -->
    <div class="col-12">
      <q-card class="fit no-shadow" bordered>
        <q-tab-panels v-model="tab" animated>
          <q-tab-panel name="contact" style="padding: 0">

            <q-list ref="scrollTargetRef" :style="'max-height: ' + alturaScroll + 'px; overflow: auto;'" class="rounded-borders" separator>
              <q-infinite-scroll ref="infinteScrollRef" @load="obterDados" :offset="alturaScroll" :scroll-target="scrollTargetRef">
                  <q-item
                    v-for="(cliente, index) in clientes"
                    :key="index"
                    clickable
                    v-ripple
                    :class="(index % 2 === 0 ? 'bg-grey-3' : 'bg-white') + ' text-black'"
                    style="margin-bottom: 2px; border-bottom: 2px solid #D4D4D4; padding: 10px !important; padding-top: 15px !important; padding-bottom: 12px ;"
                  >

                    <q-item-section @click="carregarCliente(cliente)">
                      <q-item-label lines="1" class="q-pb-xs">
                        <i class="fa-solid fa-circle-user text-black" style="font-size: 16px;"></i>
                        {{ cliente.nome }}
                      </q-item-label>
                      <q-item-label caption lines="2">
                        <span class="text-black">
                          <i class="fa-regular fa-id-card text-black" style="font-size: 15px;"></i>
                          {{ cliente.cpf }}
                        </span>
                      </q-item-label>
                    </q-item-section>

                    <q-item-section @click="carregarCliente(cliente)">
                      <q-item-label lines="1" class="q-pb-xs">
                        <i class="fa-regular fa-envelope text-black" style="font-size: 15px;"></i>
                        {{ cliente.email }}
                      </q-item-label>
                      <q-item-label caption lines="2">
                        <span class="text-black">
                          <i class="fa-solid fa-phone text-black"></i>
                          {{ cliente.telefone }}
                        </span>
                      </q-item-label>
                    </q-item-section>

                    <q-item-section side>
                      <div class="text-black q-gutter-xs">
                        <q-btn size="sm" flat dense round icon="edit" class="q-mr-xs" @click="editar(cliente)"/>
                        <q-btn size="sm" flat dense round icon="delete" class="q-mr-xs" @click="remover(cliente)"/>
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
                <q-item-label lines="1">{{ cliente_selecionado.nome }}</q-item-label>
                <q-item-label caption lines="1">
                  <span class="text-weight-bold">{{ cliente_selecionado.UltimoAcesso }}</span>
                </q-item-label>
                <q-item-label caption lines="1">
                  <q-badge v-if="!cliente_selecionado.excluido" color="blue" text-color="white" label="Ativo" />
                  <q-badge v-else color="red" text-color="white" label="Inativo" />
                </q-item-label>
              </q-item-section>
            </q-item>

            <q-space/>

            <q-btn round flat icon="edit" @click="editar(cliente_selecionado)"/>
            <q-btn round flat icon="keyboard_backspace" @click="resetar();cliente_selecionado={}"/>

          </q-toolbar>
          <q-separator></q-separator>

          <div v-for="atributo, atributo_index in atributos_cliente">
            <contact-detail-item :icon="atributo.icon" :text_color="atributo.text_color"
                                 :value="cliente_selecionado[atributo['field']]" :label="atributo.label"></contact-detail-item>

            <q-separator inset="item" v-if="atributo_index!=atributos_cliente.length-1"></q-separator>
          </div>

        </q-card>
  </transition>

  <!-- confirmacao de remocao de cliente -->
  <q-dialog v-model="confirmacaoRemoverCliente" persistent>
      <q-card>
        <q-card-section class="row items-center">
          <q-avatar icon="fas fa-user-slash" color="primary" text-color="white" />
          <span class="q-ml-sm text-justify q-pt-md">Deseja realmente remover o cliente "{{ clienteNome_ParaRemover }}"?</span>
          <!-- <span class="q-ml-sm q-mt-md text-justify"><small>Sua  cliente não deixará de ter acesso ao app, apenas reduzirá a quantidade de clientes ativos em sua assinatura. Caso seu cliente desabilitado acesse novamente ele será reativado automaticamente.</small></span> -->
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="primary" v-close-popup @click="confirmacaoRemoverCliente = false" />
          <q-btn label="Sim, quero remover" dense color="teal" v-close-popup @click="_executarRemover" />
        </q-card-actions>
      </q-card>
    </q-dialog>
</template>

<script>
import {defineComponent, defineAsyncComponent} from 'vue';
import {ref} from 'vue'
import { api } from 'boot/axios'
import { useQuasar, Loading, LocalStorage, Notify, QSpinnerGears } from 'quasar'

const atributos_cliente = [
  {
    icon: 'phone',
    label: 'Telefone',
    field: 'telefone',
    text_color: 'blue'
  },
  {
    icon: 'mail',
    label: 'Email',
    field: 'email',
    text_color: 'grey-8'
  },
   {
    icon: 'fas fa-id-card',
    label: 'CPF',
    field: 'cpf',
    text_color: 'grey-8'
  },
  {
    icon: 'fa-solid fa-map-location-dot',
    label: 'Endereço',
    field: 'endereco',
    text_color: 'grey-8'
  },
  {
    icon: 'far fa-calendar-check',
    label: 'Data de Cadastro',
    field: 'dataCadastro',
    text_color: 'grey-8'
  },
  {
    icon: 'fa-regular fa-file-lines',
    label: 'Observação',
    field: 'observacao',
    text_color: 'grey-8'
  },
];


export default defineComponent({
  name: 'TabClientes',

  mounted () {
    // this.obterDados()
  },

  components: {
    ContactDetailItem: defineAsyncComponent(() => import('components/ContactDetailItem.vue'))
  },

  setup() {
    const termoPesquisa = ref('');
    const ultimaPesquisa = ref('');
    const scrollTargetRef = ref(null)
    const alturaScroll = document.documentElement.clientHeight - 200
    const confirmacaoRemoverCliente = ref(false)
    const clienteId_ParaRemover = ref(0)
    const clienteNome_ParaRemover = ref('')

    const $q = useQuasar()
    const size = ref({ width: '200px', height: '200px' });

    return {
      scrollTargetRef,
      clientes: ref([]),
      tab: ref('contact'),
      termoPesquisa,
      ultimaPesquisa,
      pagina: 0,
      alturaScroll,
      cliente_selecionado: ref({}),
      size,
      atributos_cliente,
      confirmacaoRemoverCliente,
      clienteId_ParaRemover,
      clienteNome_ParaRemover
    }
  },

  methods: {

    async carregarCliente (cliente) {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.get(`cliente/${cliente.id}`)
        if (response.data) {
          instance.cliente_selecionado = response.data
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
      // Loading.show({ spinner: QSpinnerGears })

      this.clientes = this.pagina === 0 ? [] : this.clientes
      this.pagina = index >= 0 ? index : (this.pagina + 1)

      try {
        var finalizarCarregamento = (possuiRegistros) => {
          setTimeout(() => {
            if (possuiRegistros === false) {
              instance.$refs.infinteScrollRef.stop()
            }
            if (typeof done === "function") {
              done() // usado pra desaparecer o spinner...
            }
          }, index < 5 ? 1000 : 400)
        }

        const totalPorPagina = 12
        const response = await api.get(`cliente/${this.pagina}/${totalPorPagina}/${this.termoPesquisa}`)
        if (response.data && response.data.length > 0) {
          this.clientes.push(...response.data)
        }

        if (index >= 0) {
          finalizarCarregamento(response.data.length > 0)
        }

      } catch (error) {
        console.log(error)
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
        instance.clientes = []
        instance.$refs.infinteScrollRef.reset()
        setTimeout(() => {
          instance.$refs.infinteScrollRef.resume()
        }, 500)
      }, e === '' ? 50 : 1000)
    },

    resetar () {
      this.pagina = 0
      this.clientes = []
    },

// TODO: criar o metodo editar e o remover, recebendo o objeto cliente (chamar rota para o editar e o remover abrir um modal de confirmação)
    async editar (cliente) {
      console.log('editar', cliente)
    },

    remover (cliente) {
      this.clienteId_ParaRemover = cliente.id
      this.clienteNome_ParaRemover = cliente.nome
      this.confirmacaoRemoverCliente = true
    },

    async _executarRemover () {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.delete(`cliente/${instance.clienteId_ParaRemover}`)
        if (response.status === 200) {
          Notify.create({
            message: `Cliente removido com sucesso`,
            color: 'positive',
            icon: 'check',
            position: 'center',
            timeout: 2500
          })
          instance.clientes = []
          instance.obterDados(1)
        } else {
          Notify.create({
            message: 'Não foi possível remover o cliente, tente novamente...',
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
    },

    async atualizarStatus (cliente) {
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.post(`cliente/status`, JSON.stringify(cliente.Id))
        if (response.data && response.data.Result && response.data.Result === true) {
          cliente.Status = cliente.Status === 1 ? 2 : 1
          Notify.create({
            message: `Cliente ${ cliente.Status === 1 ? 'ativado' : 'desativado'} com sucesso`,
            color: 'positive',
            icon: 'check',
            position: 'bottom',
            timeout: 1000
          })

        } else {
          Notify.create({
            message: 'Não foi possível atualizar o cliente, tente novamente...',
            color: 'negative',
            icon: 'warning',
            position: 'center',
            timeout: 2500
          })
        }


      } catch (error) {
        Notify.create({
          message: 'Ops! Não foi possível atualizar o cliente, tente novamente mais tarde...',
          color: 'negative',
          icon: 'warning',
          position: 'center',
          timeout: 2500
        })
        console.log(error)
      }

      Loading.hide()

    }
  }
})
</script>
<!-- <style>
img::after {
     content: "\2639" " " attr(alt);

     font-size: 30px;
     color: rgb(100, 100, 100);

     display: block;
     position: absolute;
     z-index: 2;
     top: 30%;
     left: 25%;
     width: 100%;
     height: 100%;
}
</style> -->
