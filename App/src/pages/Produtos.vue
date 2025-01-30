<template>
  <q-page class="q-pa-sm">
    <div class="row" v-if="Object.keys(produto_selecionado).length==0">
      <div class="col-12">
        <q-card class="no-border no-shadow bg-transparent">
          <q-card-section class="q-pa-xs">
            <q-input rounded v-model="termoPesquisa" @update:model-value="pesquisar" outlined placeholder="Pesquisar por nome do produto">
              <template v-slot:append>
                <q-icon v-if="termoPesquisa === ''" name="search"/>
                <q-icon v-else name="clear" class="cursor-pointer" @click="termoPesquisa = '';resetar();pesquisar('')"/>
              </template>
            </q-input>
          </q-card-section>
        </q-card>
      </div>

      <div v-if="produtos.length === 0" class="col-12 text-center q-pa-md">
        <q-card class="no-border no-shadow bg-warning text-white">
          <q-card-section>
            <q-icon name="info" size="lg" />
            <span>Nenhum produto para exibir.</span>
          </q-card-section>
        </q-card>
      </div>

      <!-- listagem de produtos com rolagem infinita -->
      <div class="col-12">
        <q-card class="fit no-shadow" bordered>
          <q-tab-panels v-model="tab" animated>
            <q-tab-panel name="contact" style="padding: 0">

              <q-list ref="scrollTargetRef" :style="'max-height: ' + alturaScroll + 'px; overflow: auto;'" class="rounded-borders" separator>
                <q-infinite-scroll ref="infinteScrollRef" @load="obterDados" :offset="alturaScroll" :scroll-target="scrollTargetRef">
                    <q-item
                      v-for="(produto, index) in produtos"
                      :key="index"
                      clickable
                      v-ripple
                      :class="(index % 2 === 0 ? 'bg-grey-3' : 'bg-white') + ' text-black'"
                      style="margin-bottom: 2px; border-bottom: 2px solid #D4D4D4; padding: 10px !important; padding-top: 15px !important; padding-bottom: 0"
                    >

                      <q-item-section @click="carregar(produto)">
                        <q-item-label lines="1" class="q-pb-xs" style="font-weight: 500;">
                          <!-- <i class="fa-solid fa-circle-user text-black" style="font-size: 16px;"></i> -->
                          {{ produto.nome }}
                        </q-item-label>

                        <div class="row">
                          <div class="col-6 text-black">
                            <!-- <i class="fa-regular fa-id-card text-black" style="font-size: 15px;"></i> -->
                            Estoque: {{ produto.quantidadeEstoque }}
                          </div>
                          <div class="col-6">
                            Valor: {{ formatarParaReal(produto.valorVenda) }}
                          </div>
                        </div>

                      </q-item-section>



                      <q-item-section side>
                        <div class="text-black q-gutter-xs">
                          <q-btn size="sm" flat dense round icon="edit" class="q-mr-xs" @click="editar(produto)"/>
                          <q-btn size="sm" flat dense round icon="delete" class="q-mr-xs" @click="remover(produto)"/>
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
              <q-btn round class="q-pa-sm q-mt-md q-mb-md q-mr-md q-ml-sm text-teal" icon="fa-solid fa-cube" size="lg"></q-btn>

              <q-item class="q-subtitle-1 q-pl-md">
                <q-item-section>
                  <q-item-label lines="1">{{ produto_selecionado.nome }}</q-item-label>
                  <q-item-label caption lines="1">
                    <!-- <q-badge v-if="!produto_selecionado.excluido" color="blue" text-color="white" label="Ativo" />
                    <q-badge v-else color="red" text-color="white" label="Inativo" /> -->
                  </q-item-label>
                </q-item-section>
              </q-item>

              <q-space/>

              <q-btn round flat icon="edit" @click="editar(produto_selecionado)"/>
              <q-btn round flat icon="keyboard_backspace" @click="resetar();produto_selecionado={}"/>

            </q-toolbar>
            <q-separator></q-separator>

            <div v-for="atributo, atributo_index in atributos" :key="atributo_index">
              <contact-detail-item :icon="atributo.icon" :text_color="atributo.text_color"
                                  :value="produto_selecionado[atributo['field']]" :label="atributo.label" :expand="atributo.expand"></contact-detail-item>

              <q-separator inset="item" v-if="atributo_index!=atributos.length-1"></q-separator>
            </div>

          </q-card>
    </transition>

    <!-- confirmacao de remocao -->
    <q-dialog v-model="confirmacaoRemover" persistent>
      <q-card>
        <q-card-section class="row items-center">
          <q-avatar icon="fas fa-user-slash" color="primary" text-color="white" />
          <span class="q-ml-sm text-justify q-pt-md">Deseja realmente remover o produto "{{ nome_ParaRemover }}"?</span>
        </q-card-section>

        <q-card-actions align="right">
          <q-btn flat label="Cancelar" color="primary" v-close-popup @click="confirmacaoRemover = false" />
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
          to="/produto/novo"
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

const atributos = [
  {
    icon: 'text_fields',
    label: 'Nome',
    field: 'nome',
    text_color: 'blue'
  },
  {
    icon: 'fa-solid fa-dollar-sign',
    label: 'Valor de Venda',
    field: 'valorVenda',
    text_color: 'green-8'
  },
   {
    icon: 'fa-solid fa-dollar-sign',
    label: 'Valor de Custo',
    field: 'valorCusto',
    text_color: 'red-8'
  },
  {
    icon: 'fa-solid fa-cube',
    label: 'Estoque',
    field: 'quantidadeEstoque',
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
    text_color: 'grey-8',
    expand: true
  },
];

export default defineComponent({
  name: 'Produtos',
  components: {
    ContactDetailItem: defineAsyncComponent(() => import('components/ContactDetailItem.vue'))
  },

  setup() {
    const termoPesquisa = ref('');
    const ultimaPesquisa = ref('');
    const scrollTargetRef = ref(null)
    const alturaScroll = document.documentElement.clientHeight - 200
    const confirmacaoRemover = ref(false)
    const id_ParaRemover = ref(0)
    const nome_ParaRemover = ref('')

    const $q = useQuasar()
    const size = ref({ width: '200px', height: '200px' });

    return {
      scrollTargetRef,
      produtos: ref([]),
      tab: ref('contact'),
      termoPesquisa,
      ultimaPesquisa,
      pagina: 0,
      alturaScroll,
      produto_selecionado: ref({}),
      size,
      atributos,
      confirmacaoRemover,
      id_ParaRemover,
      permissaoEdicao: ref(false)
    }
  },

  mounted () {
    var permitidoEdicao = [ 99, 2 ]
    this.permissaoEdicao = permitidoEdicao.indexOf(LocalStorage.getItem('perfilPermissao')) > -1
  },

  methods: {

    formatarParaReal (valor) {
      return new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' }).format(valor);
    },

    async carregar (modelo) {
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.get(`produto/${modelo.id}`)
        this.produto_selecionado = response.data ?? []
      } catch (error) {
        // console.log(error)
      }
      finally {
        Loading.hide()
      }
    },

    async obterDados (index, done) {
      var instance = this
      // Loading.show({ spinner: QSpinnerGears })

      this.produtos = this.pagina === 0 ? [] : this.produtos
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
        const response = await api.get(`produto/${this.pagina}/${totalPorPagina}/${this.termoPesquisa}`)
        if (response.data && response.data.length > 0) {
          this.produtos.push(...response.data)
        }

        if (index >= 0) {
          finalizarCarregamento(response.data.length > 0)
        }

      } catch (error) {
        // console.log(error)
      }

    },

    pesquisar (e) {
      var instance = this
      this.pagina = 1

      setTimeout(async () => {
        if (e === instance.termoPesquisa && instance.termoPesquisa !== '') {
          instance.ultimaPesquisa = instance.termoPesquisa
          // instance.$refs.infinteScrollRef.resume()
        } else if (e === '' && instance.ultimaPesquisa !== '') {
          instance.termoPesquisa = ''
          instance.ultimaPesquisa = ''
          // instance.$refs.infinteScrollRef.resume()
        }
        instance.produtos = []
        instance.$refs.infinteScrollRef.reset()
        setTimeout(() => {
          instance.$refs.infinteScrollRef.resume()
        }, 500)
      }, e === '' ? 50 : 1000)
    },

    resetar () {
      this.pagina = 0
      this.produtos = []
    },

    editar (modelo) {
      if (this.permissaoEdicao === false) {
        Notify.create({
          message: 'Você não tem permissão para editar produtos',
          color: 'negative',
          icon: 'warning',
          position: 'center',
          timeout: 2000
        })
        return
      }
      this.$router.push(`/produto/${modelo.id}`)
    },

    remover (modelo) {
      if (this.permissaoEdicao === false) {
        Notify.create({
          message: 'Você não tem permissão para remover produtos',
          color: 'negative',
          icon: 'warning',
          position: 'center',
          timeout: 2000
        })
        return
      }
      this.id_ParaRemover = modelo.id
      this.nome_ParaRemover = modelo.nome
      this.confirmacaoRemover = true
    },

    async _executarRemover () {
      var instance = this
      Loading.show({ spinner: QSpinnerGears })

      try {
        const response = await api.delete(`produto/${instance.id_ParaRemover}`)
        if (response.status === 200) {
          Notify.create({
            message: `Produto removido com sucesso`,
            color: 'positive',
            icon: 'check',
            position: 'center',
            timeout: 2500
          })
          instance.produtos = []
          instance.obterDados(1)
        } else {
          Notify.create({
            message: 'Não foi possível remover, tente novamente...',
            color: 'negative',
            icon: 'warning',
            position: 'center',
            timeout: 2500
          })
        }
      } catch (error) {
        // console.log(error)
      }
      finally {
        Loading.hide()
      }
    }
  }
})
</script>
