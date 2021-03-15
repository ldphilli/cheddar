<template>
  <card>
    <h5 slot="header" class="title">Add Stock</h5>
    <div class="row">
      <div class="col-md-3 px-md-1">
        <base-input label="Company Name"
                  placeholder="Company name"
                  v-model="model.companyName">
        </base-input>
      </div>
      <div class="col-md-5 pr-md-1">
        <base-input label="Ticker Symbol"
                  placeholder="Ticker symbol"
                  v-model="model.tickerSymbol">
        </base-input>
      </div>
    </div>
    <base-button slot="footer" type="primary" fill>Save</base-button>
  </card>
</template>
<script>
  export default {
    props: {
      model: {
        type: Object,
        default: () => {
          return {};
        }
      }
    }
  }
</script>
<style>
</style>
