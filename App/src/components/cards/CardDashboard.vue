<template>
  <q-card class="bg-transparent no-shadow no-border" bordered>
    <q-card-section class="q-pa-none">
      <div class="row q-col-gutter-sm ">
        <div v-for="(item, index) in items" :key="index" class="col-md-3 col-sm-12 col-xs-12">
          <q-item :style="`background-color: ${item.color1}`" class="q-pa-none" to="/clientes" >
            <q-item-section v-if="icon_position === 'left'" side :style="`background-color: ${item.color2}`"
                            class=" q-pa-lg q-mr-none text-white">
              <q-icon :name="item.icon" color="white" size="24px"></q-icon>
            </q-item-section>
            <q-item-section class=" q-pa-md q-ml-none  text-white">
              <q-item-label class="text-white text-h6 text-weight-bolder">{{ item.value }}</q-item-label>
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
    this.obterDados()
  },

  computed: {
    items: function () {
      return [
          {
            title: "Clientes Ativos",
            icon: "fas fa-users",
            value: this.dadosDashboard.ClientesAtivos,
            color1: "#5064b5",
            color2: "#3e51b5"
          },
          {
            title: "Clientes Inativos",
            icon: "fas fa-users",
            value: this.dadosDashboard.ClientesInativos,
            color1: "#f37169",
            color2: "#f34636"
          },
          {
            title: "Últimos acessos (ÚNICOS)",
            icon: "bar_chart",
            value: this.dadosDashboard.TotalAcessosUnicos,
            color1: "#ea6a7f",
            color2: "#ea4b64"
          },
          {
            title: "Últimos acessos (TOTAL)",
            icon: "bar_chart",
            value: this.dadosDashboard.TotalAcessos,
            color1: "#a270b1",
            color2: "#9f52b1"
          }
        ]
    }
  },

  setup() {
    const dadosDashboard = ref({
      ClientesAtivos: 0,
      ClientesInativos: 0,
      TotalAcessosUnicos: 0,
      TotalAcessos: 0,
      AcessosUnicos: [],
      Acessos: []
    });

    return {
      dadosDashboard
    }
  },

  methods: {
    async obterDados () {
      Loading.show({ spinner: QSpinnerGears })
      try {
        const response = await api.post(`empresa/dashboard`)
        this.dadosDashboard = response.data.Result
      } catch (error) {
        console.log(error)
      }
      Loading.hide()
    }
  }
})
</script>
