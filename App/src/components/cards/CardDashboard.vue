<template>
  <q-card class="bg-transparent no-shadow no-border" bordered>
    <q-card-section class="q-pa-none">
      <div class="row q-col-gutter-sm ">
        <div v-for="(item, index) in items" :key="index" class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
          <q-item  clickable v-ripple :style="`background-color: ${item.color1}`" class="q-pa-none" :to="`${item.destino}`" >
            <q-item-section v-if="icon_position === 'left'" side :style="`background-color: ${item.color2}`"
                            class=" q-pa-lg q-mr-none text-white">
              <q-icon :name="item.icon" color="white" size="24px"></q-icon>
            </q-item-section>
            <q-item-section class=" q-pa-md q-ml-none text-white">
              <div v-if="item.carregando === true" class="text-white text-center" style="height: 10px !important;">
                <q-spinner-dots color="white" style="margin: 0; padding: 0; " size="40px" />
              </div>
              <q-item-label v-else class="text-white text-h6 text-weight-bolder">
                {{ item.value }}
              </q-item-label>
              <q-item-label>{{ item.title }}</q-item-label>
            </q-item-section>
            <q-item-section v-if="icon_position === 'right'" side class="q-mr-md text-white">
              <q-icon :name="item.icon" color="white" size="44px"></q-icon>
            </q-item-section>
          </q-item>
        </div>
      </div>
    </q-card-section>
  </q-card>
</template>

<script>
import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'
import { Loading, QSpinnerGears } from 'quasar'

export default defineComponent({
  name: "CardDashboard",
  props: {
    icon_position: {
      required: false,
      default: "left"
    }
  },

  mounted () {
    this.obterTotalClientes()
    this.obterTotalProdutos()
  },

  computed: {
    items: function () {
      return [
          {
            title: this.dadosDashboard.TotalDeClientes > 1 ? "Clientes" : "Cliente",
            icon: "fas fa-users",
            value: this.dadosDashboard.TotalDeClientes,
            color1: "#5064b5",
            color2: "#3e51b5",
            carregando: this.dadosDashboard.CarregandoTotalDeClientes,
            destino: "/clientes"
          },
          {
            title: this.dadosDashboard.TotalDeProdutos > 1 ? "Produtos" : "Produto",
            icon: "fas fa-solid fa-cubes",
            value: this.dadosDashboard.TotalDeProdutos,
            color1: "#f37169",
            color2: "#f34636",
            carregando: this.dadosDashboard.CarregandoTotalDeProdutos,
            destino: "/produtos"
          }
        ]
    }
  },

  setup() {
    const dadosDashboard = ref({
      TotalDeClientes: 0,
      TotalDeProdutos: 0,
      CarregandoTotalDeClientes: true,
      CarregandoTotalDeProdutos: true
    });

    return {
      dadosDashboard
    }
  },

  methods: {
    async obterTotalClientes () {
      this.dadosDashboard.CarregandoTotalDeClientes = true
      try {
        const response = await api.get(`cliente/dashboard`)
        this.dadosDashboard.TotalDeClientes = response.data
      } catch (error) {
        console.log(error)
      }
      finally {
        this.dadosDashboard.CarregandoTotalDeClientes = false
      }
    },

    async obterTotalProdutos () {
      try {
        const response = await api.get(`produto/dashboard`)
        this.dadosDashboard.TotalDeProdutos = response.data
      } catch (error) {
        console.log(error)
      }
      finally {
        this.dadosDashboard.CarregandoTotalDeProdutos = false
      }
    }
  }
})
</script>
