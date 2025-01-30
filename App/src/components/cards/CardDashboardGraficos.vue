<template>
    <!-- produtos com maior estoque -->
    <div class="row q-col-gutter-sm q-py-sm q-pr-sm">
      <q-card class="col-lg-8 col-md-8 col-sm-12 col-xs-12 q-ma-sm">
        <q-card-section>
          <div class="text-h6">Top 10 Produtos com Maior Estoque</div>
          <ECharts
            v-if="!maiorEstoqueEstaVazio"
            ref="barchartMaiorEstoque"
            :option="barChartMaiorEstoqueOptions"
            style="height: 400px;"
            :resizable="true"
            autoresize
            class="q-mt-md" />
          <div v-else class="text-h6 text-center">
            <q-card-section class="bg-red-1 text-green-10 text-center q-pa-md">
            Nenhum produto com estoque positivo.
            </q-card-section>
          </div>
        </q-card-section>
      </q-card>
    </div>

    <!-- produtos negativos -->
    <div class="row q-col-gutter-sm q-py-sm q-pr-sm">
      <q-card class="col-lg-8 col-md-8 col-sm-12 col-xs-12 q-ma-sm">
        <q-card-section style="padding: 0">
          <div class="text-h6 text-center">Produtos com Estoque Zerado ou Negativo</div>
          <ECharts
            v-if="estoqueNegativoEstaVazio"
            ref="barChartEstoqueZeradoOuNegativo"
            :option="barChartEstoqueZeradoOuNegativoOptions"
            style="height: 400px;"
            :resizable="true"
            autoresize
            class="q-mt-md" />
          <div v-else class="text-h6 text-center">
            <q-card-section class="bg-green-1 text-green-10 text-center q-pa-md">
            Todos os produtos estão com estoque positivo.
            </q-card-section>
          </div>

        </q-card-section>
      </q-card>
    </div>
</template>

<script>
import {defineComponent, ref} from 'vue'
import { api } from 'boot/axios'

import ECharts from 'vue-echarts'

// Importações necessárias do ECharts
import * as echarts from 'echarts/core'
import { BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent
} from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'

// Registrar componentes
echarts.use([
  TitleComponent,
  TooltipComponent,
  GridComponent,
  LegendComponent,
  BarChart,
  CanvasRenderer
])


export default defineComponent({
  name: "CardDashboardGraficos",

  components: {
    ECharts
  },

  mounted () {
    this.obterProdutosComEstoqueZeradoOuNegativo()
    this.obterTopDezProdutosComMaiorEstoque()
  },

  setup() {
    const barChartMaiorEstoqueOptions = ref({
      xAxis: {
        type: 'category',
        data: []
      },
      yAxis: {
        type: 'value'
      },
      series: [{
        data: [],
        type: 'bar',
        itemStyle: {
          color: function(params) {
            // Array de cores que serão usadas ciclicamente
            const colorList = [
              '#5470c6',
              '#91cc75',
              '#fac858',
              '#ee6666',
              '#73c0de',
              '#3ba272',
              '#fc8452',
              '#9a60b4',
              '#ea7ccc',
              '#4ec1bf'
            ];
            return colorList[params.dataIndex % colorList.length];
          }
        }
      }],
      tooltip: {
        trigger: 'axis'
      }
    })


    const barChartEstoqueZeradoOuNegativoOptions = ref({
      xAxis: {
        type: 'category',
        data: []
      },
      yAxis: {
        type: 'value'
      },
      series: [{
        data: [],
        type: 'bar',
        itemStyle: {
          color: function(params) {
            // Array de cores que serão usadas ciclicamente
            const colorList = [
              '#5470c6',
              '#91cc75',
              '#fac858',
              '#ee6666',
              '#73c0de',
              '#3ba272',
              '#fc8452',
              '#9a60b4',
              '#ea7ccc',
              '#4ec1bf'
            ];
            return colorList[params.dataIndex % colorList.length];
          }
        }
      }],
      tooltip: {
        trigger: 'axis'
      }
    })

    return {
      barChartMaiorEstoqueOptions,
      barChartEstoqueZeradoOuNegativoOptions,
      maiorEstoqueEstaVazio: false,
      estoqueNegativoEstaVazio: false,
      mode: 'list'
    }
  },

  methods: {
    async obterProdutosComEstoqueZeradoOuNegativo () {
      try {
        const response = await api.get(`produto/estoque-negativo`)
        this.estoqueNegativoEstaVazio = response.data ? response.data.length === 0 : true
        this.barChartEstoqueZeradoOuNegativoOptions.xAxis.data = []
        this.barChartEstoqueZeradoOuNegativoOptions.series[0].data = []

        if (response.data) {
          for (let i = 0; i < response.data.length; i++) {
            this.barChartEstoqueZeradoOuNegativoOptions.xAxis.data.push(response.data[i].key)
            this.barChartEstoqueZeradoOuNegativoOptions.series[0].data.push(response.data[i].value)
          }
        }
      } catch (error) {
        console.log(error)
      }
    },

    async obterTopDezProdutosComMaiorEstoque () {
      try {
        const response = await api.get(`produto/top`)
        this.maiorEstoqueEstaVazio = response.data ? response.data.length === 0 : true
        this.barChartMaiorEstoqueOptions.xAxis.data = []
        this.barChartMaiorEstoqueOptions.series[0].data = []

        if (response.data) {
          for (let i = 0; i < response.data.length; i++) {
            this.barChartMaiorEstoqueOptions.xAxis.data.push(response.data[i].key)
            this.barChartMaiorEstoqueOptions.series[0].data.push(response.data[i].value)
          }
        }
      } catch (error) {
        console.log(error)
      }
    }
  }
})
</script>
