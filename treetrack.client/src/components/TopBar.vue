<template>
  <div class="topbar">
    <div class="logo-brand">
      <img src="/logo.png" alt="" class="logo-img" />
      <div class="logo-text">Tree<span>Track</span></div>
    </div>
    <div class="topbar-sep"></div>
    <div class="breadcrumb">
      <slot name="breadcrumb" />
    </div>
    <div class="topbar-actions">
      <slot name="actions" />
    </div>
  </div>
</template>
