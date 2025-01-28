<template>
  <q-page class="q-pa-sm">
    <card-dashboard />

    <!-- <card-charts/> -->

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
    CardCharts: defineAsyncComponent(() => import('components/cards/CardCharts.vue'))
    // TabSocial: defineAsyncComponent(() => import('components/tabs/TabSocial.vue')),
    // CardWithImage: defineAsyncComponent(() => import('components/cards/CardWithImage.vue')),
    // CardTimeLine: defineAsyncComponent(() => import('components/cards/CardTimeLine.vue')),
    // TodoList: defineAsyncComponent(() => import('components/list/TodoList.vue')),
    // TableVisits: defineAsyncComponent(() => import('components/tables/TableVisits.vue')),
  },
  setup() {
    return {
      mode: 'list'
    }
  },

})
</script>
