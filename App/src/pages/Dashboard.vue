<template>
  <q-page class="q-pa-sm">
    <card-dashboard />

    <card-dashboard-graficos />
    <!-- <card-charts/> -->

    <!-- <div class="row q-col-gutter-sm q-py-sm q-pr-sm">
      <q-card class="col-lg-8 col-md-8 col-sm-12 col-xs-12 q-ma-sm">
        <q-card-section>
          <div class="text-h6">Top 10 Produtos com Maior Estoque</div>
          <ECharts
            ref="barchartMaiorEstoque"
            :option="barChartOptions"
            style="height: 400px;"
            :resizable="true"
            autoresize
            class="q-mt-md" />
        </q-card-section>
      </q-card>
    </div> -->

    <!-- <div class="row q-col-gutter-sm  q-py-sm">
      <tab-social/>
      <card-with-image/>
    </div> -->

    <!-- <div class="row q-col-gutter-sm  q-pb-sm">
      <todo-list/>

      <card-time-line/>
    </div> -->

    <!-- <table-visits/> -->
  </q-page>
</template>

<script>
import {defineComponent,defineAsyncComponent} from 'vue'

export default defineComponent({
  name: 'PageIndex',
  components: {
    CardDashboard: defineAsyncComponent(() => import('components/cards/CardDashboard.vue')),
    CardDashboardGraficos: defineAsyncComponent(() => import('components/cards/CardDashboardGraficos.vue')),
  },

  setup() {
    return {
      mode: 'list'
    }
  },

})
</script>
