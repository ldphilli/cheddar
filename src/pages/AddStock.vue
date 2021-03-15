<template>
  <div class="row">
    <div class="col-md-8">
      <edit-stock-form :model="model">
      </edit-stock-form>
    </div>
  </div>
</template>
<script>
  import EditStockForm from './Stocks/EditStockForm'
  export default {
    components: {
      EditStockForm,
    },
    data() {
      return {
        model: {
          companyName: 'Legal & General',
          tickerSymbol: 'LGEN'
        }
      }
    }
  }
</script>
<style>
</style>
